package com.hutech.easylearning.service;


import com.hutech.easylearning.dto.reponse.AuthenticationResponse;
import com.hutech.easylearning.dto.reponse.GoogleUserInfo;
import com.hutech.easylearning.dto.reponse.IntrospectResponse;
import com.hutech.easylearning.dto.request.*;
import com.hutech.easylearning.entity.InvalidatedToken;
import com.hutech.easylearning.entity.User;
import com.hutech.easylearning.exception.AppException;
import com.hutech.easylearning.exception.ErrorCode;
import com.hutech.easylearning.repository.InvalidatedTokenRepository;
import com.hutech.easylearning.repository.RoleRepository;
import com.hutech.easylearning.repository.UserRepository;
import com.nimbusds.jose.*;
import com.nimbusds.jose.crypto.MACSigner;
import com.nimbusds.jose.crypto.MACVerifier;
import com.nimbusds.jwt.JWTClaimsSet;
import com.nimbusds.jwt.SignedJWT;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import lombok.experimental.NonFinal;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.security.crypto.bcrypt.BCryptPasswordEncoder;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.util.CollectionUtils;

import java.text.ParseException;
import java.time.Instant;
import java.time.LocalDateTime;
import java.time.temporal.ChronoUnit;
import java.util.*;



@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
@Slf4j
public class AuthenticationService {
    final UserRepository userRepository;
    final RoleRepository roleRepository;
    final InvalidatedTokenRepository invalidatedTokenRepository;
    final GoogleAuthService googleAuthService;

    @NonFinal
    @Value("${jwt.signerKey}")
    protected String SIGNER_KEY;

    @NonFinal
    @Value("${jwt.valid-duration}")
    protected long VALID_DURATION;

    @NonFinal
    @Value("${jwt.refreshable-duration}")
    protected long REFRESHABLE_DURATION;

    @Value("${google.token-uri}")
    private String tokenUri;

    @Value("${google.user-info-uri}")
    private String userInfoUri;

    @Value("${google.client-id}")
    private String clientId;

    @Value("${google.client-secret}")
    private String clientSecret;

    @Value("${google.redirect-uri}")
    private String redirectUri;

    @NonFinal
    protected final String GRANT_TYPE = "authorization_code";

    public IntrospectResponse introspect(IntrospectRequest request) throws JOSEException, ParseException {
        var token = request.getToken();
        boolean isValid = true;
        try {
            verifyToken(token, false);
        } catch (AppException e) {
            isValid = false;
        }
        return IntrospectResponse.builder()
                .valid(isValid)
                .build();
    }

    public AuthenticationResponse authenticateGoogle(String accessToken) {
        GoogleUserInfo userInfo = googleAuthService.verifyAccessToken(accessToken);
        if (userInfo != null) {
            User user = userRepository.findByEmail(userInfo.getEmail())
                    .orElseGet(() -> {
                        User newUser = new User();
                        String userImageUrl = "http://res.cloudinary.com/dofr3xzmi/image/upload/v1720255836/aoy4tixw5shd9cxh5ep1.jpg";
                        if(userInfo.getPicture() != null)
                        {
                            userImageUrl = userInfo.getPicture();
                        }
                        newUser.setImageUrl(userImageUrl);
                        newUser.setDateCreate(LocalDateTime.now());
                        newUser.setDateChange(LocalDateTime.now());
                        newUser.setIsDeleted(false);
                        newUser.setEmail(userInfo.getEmail());
                        newUser.setFullName(userInfo.getName());
                        newUser.setPassword("");
                        newUser.setUserName(userInfo.getEmail());
                        List<String> defaultRoleNames = List.of("USER");
                        var roles = roleRepository.findByNameIn(defaultRoleNames);
                        newUser.setRoles(new HashSet<>(roles));
                        return newUser;
                    });

            String token = generateToken(user);
            return AuthenticationResponse.builder().token(token).authenticated(true).build();
        } else {
            throw new RuntimeException("User info not found from Google ID token");
        }
    }
    public AuthenticationResponse authenticate(AuthenticationRequest request)
    {
        var user = userRepository.findByUserName(request.getUserName())
                .orElseThrow(() -> new AppException(ErrorCode.USER_NOT_EXISTED));
        if(user.getIsDeleted())
        {
            throw new AppException(ErrorCode.USER_BLOCKED);
        }
        PasswordEncoder passwordEncoder = new BCryptPasswordEncoder(10);
        boolean authenticated = passwordEncoder.matches(request.getPassword(), user.getPassword());
        if(!authenticated)
        {
            throw new AppException(ErrorCode.UNCATEGORIZED_EXCEPTION);
        }

        var token = generateToken(user);
        return AuthenticationResponse.builder().token(token).authenticated(true).build();
    }

    public void logout(LogoutRequest request) throws ParseException, JOSEException {
       try{
           var signToken = verifyToken(request.getToken(), true);
           String jit = signToken.getJWTClaimsSet().getJWTID();
           Date expiryTime = signToken.getJWTClaimsSet().getExpirationTime();
           InvalidatedToken invalidatedToken = InvalidatedToken.builder()
                   .id(jit)
                   .expiryTime(expiryTime)
                   .build();
           invalidatedTokenRepository.save(invalidatedToken);
       }catch (AppException exception){
           log.info("Token already expired");

       }


    }

    public AuthenticationResponse refreshToken (RefreshRequest  request) throws ParseException, JOSEException {
        var signedJWT = verifyToken(request.getToken(), true);
        String jit = signedJWT.getJWTClaimsSet().getJWTID();
        Date expiryTime = signedJWT.getJWTClaimsSet().getExpirationTime();
        InvalidatedToken invalidatedToken = InvalidatedToken.builder()
                .id(jit)
                .expiryTime(expiryTime)
                .build();
        invalidatedTokenRepository.save(invalidatedToken);

        var userName = signedJWT.getJWTClaimsSet().getSubject();
        var user = userRepository.findByUserName(userName).orElseThrow(
                () -> new AppException((ErrorCode.UNAUTHENTICATED)));

        var token = generateToken(user);
        return AuthenticationResponse.builder().token(token).authenticated(true).build();
    }

    private SignedJWT verifyToken(String token, boolean isRefresh) throws JOSEException, ParseException
    {
        JWSVerifier verifier = new MACVerifier(SIGNER_KEY.getBytes());

        SignedJWT signedJWT = SignedJWT.parse(token);

        Date expiryTime = (!isRefresh) ? signedJWT.getJWTClaimsSet().getExpirationTime()
                : new Date(signedJWT.getJWTClaimsSet()
                .getIssueTime().toInstant().plus(REFRESHABLE_DURATION, ChronoUnit.SECONDS)
                .toEpochMilli());

        var verified = signedJWT.verify(verifier);
        if (!(verified && expiryTime.after(new Date())))
            throw new AppException(ErrorCode.UNAUTHENTICATED);

        if(invalidatedTokenRepository.existsById(signedJWT.getJWTClaimsSet().getJWTID()))
            throw new AppException(ErrorCode.UNAUTHENTICATED);

        return signedJWT;
    }
    private String generateToken(User user)
    {
        JWSHeader header = new JWSHeader(JWSAlgorithm.HS512);
        JWTClaimsSet jwtClaimsSet = new JWTClaimsSet.Builder()
                .subject(user.getUserName())
                .issuer("devteria.com")
                .issueTime(new Date())
                .expirationTime(new Date(
                        Instant.now().plus(VALID_DURATION, ChronoUnit.SECONDS).toEpochMilli()
                ))
                .jwtID(UUID.randomUUID().toString())
                .claim("scope", buildScope(user))
                .build();
        Payload payload = new Payload(jwtClaimsSet.toJSONObject());
        JWSObject jwsObject = new JWSObject(header, payload);
        try {
            jwsObject.sign(new MACSigner(SIGNER_KEY.getBytes()));
            return jwsObject.serialize();
        }
        catch (JOSEException e)
        {
            //log.error("Cannot create token", e);
            throw new RuntimeException(e);
        }
    }
    private String buildScope(User user)
    {
        StringJoiner stringJoiner = new StringJoiner(" ");
        if(!CollectionUtils.isEmpty(user.getRoles()))
        {
           user.getRoles().forEach(role -> {
               stringJoiner.add(role.getName());

           });
        }
        return stringJoiner.toString();
    }
}

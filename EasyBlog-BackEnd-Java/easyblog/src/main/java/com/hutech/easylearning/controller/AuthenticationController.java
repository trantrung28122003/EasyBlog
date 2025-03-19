package com.hutech.easylearning.controller;

import com.hutech.easylearning.dto.reponse.IntrospectResponse;
import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.*;
import com.hutech.easylearning.dto.reponse.AuthenticationResponse;
import com.hutech.easylearning.service.AuthenticationService;
import com.hutech.easylearning.service.UserService;
import com.nimbusds.jose.JOSEException;
import jakarta.validation.Valid;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.text.ParseException;
import java.util.Map;

@RestController
@RequestMapping("/auth")
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE, makeFinal = true)
public class AuthenticationController {
    @Autowired
    AuthenticationService authenticationService;

    @Autowired
    UserService userService;

    @PostMapping("/login")
    public ApiResponse<AuthenticationResponse> authenticate (@RequestBody AuthenticationRequest request)
    {
        var result = authenticationService.authenticate(request);
        return ApiResponse.<AuthenticationResponse>builder()
                .result(result)
                .build();
    }

    @PostMapping("/loginWithGoogle")
    public ApiResponse<AuthenticationResponse> authenticateWithGoogle(@RequestBody Map<String, String> payload) {
        String access_token = payload.get("access_token");
        var result = authenticationService.authenticateGoogle(access_token);
        return ApiResponse.<AuthenticationResponse>builder()
                .result(result)
                .build();
    }

    @PostMapping("/register")
    public ApiResponse<UserResponse> CreateUser(@Valid UserCreationRequest request, @RequestParam(value = "file", required = false) MultipartFile file) {
        return ApiResponse.<UserResponse>builder()
                .result(userService.createUser(request, file))
                .build();
    }

    @PostMapping("/introspect")
    ApiResponse<IntrospectResponse> authenticate (@RequestBody IntrospectRequest request) throws ParseException, JOSEException
    {
        var result = authenticationService.introspect(request);
        return ApiResponse.<IntrospectResponse>builder()
                .result(result)
                .build();
    }

    @PostMapping("/logout")
    ApiResponse<Void> logout (@RequestBody LogoutRequest request) throws ParseException, JOSEException
    {
        authenticationService.logout(request);
        return ApiResponse.<Void>builder()
                .build();
    }

    @PostMapping("/refresh")
    ApiResponse<AuthenticationResponse> authenticate (@RequestBody RefreshRequest request) throws ParseException, JOSEException
    {
        var result = authenticationService.refreshToken(request);
        return ApiResponse.<AuthenticationResponse>builder()
                .result(result)
                .build();
    }


}

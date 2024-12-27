package com.hutech.easylearning.service;



import com.hutech.easylearning.dto.reponse.GoogleUserInfo;
import com.hutech.easylearning.entity.User;
import com.hutech.easylearning.repository.UserRepository;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import org.springframework.web.client.HttpClientErrorException;
import org.springframework.http.HttpEntity;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpMethod;
import org.springframework.http.ResponseEntity;

@Service
public class GoogleAuthService {

    public GoogleUserInfo verifyAccessToken(String accessToken) {
        try {
            String url = "https://www.googleapis.com/oauth2/v1/userinfo?access_token=" + accessToken;
            RestTemplate restTemplate = new RestTemplate();
            ResponseEntity<GoogleUserInfo> response = restTemplate.exchange(
                    url, HttpMethod.GET, HttpEntity.EMPTY, GoogleUserInfo.class);
            return response.getBody();
        } catch (HttpClientErrorException e) {
            throw new RuntimeException("Invalid access token", e);
        }
    }

}


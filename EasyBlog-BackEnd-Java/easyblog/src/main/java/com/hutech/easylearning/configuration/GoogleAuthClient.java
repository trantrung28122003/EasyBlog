package com.hutech.easylearning.configuration;

import com.hutech.easylearning.dto.reponse.GoogleUserInfo;
import com.hutech.easylearning.dto.request.ExchangeTokenRequest;
import lombok.RequiredArgsConstructor;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Component;
import org.springframework.web.client.RestTemplate;

@Component
@RequiredArgsConstructor
public class GoogleAuthClient {

    private final RestTemplate restTemplate;

    public String exchangeToken(ExchangeTokenRequest request, String tokenUri) {
        return restTemplate.postForObject(tokenUri, request, String.class);
    }

    public GoogleUserInfo getUserInfo(String accessToken, String userInfoUri) {
        return restTemplate.getForObject(userInfoUri + "?access_token=" + accessToken, GoogleUserInfo.class);
    }
}

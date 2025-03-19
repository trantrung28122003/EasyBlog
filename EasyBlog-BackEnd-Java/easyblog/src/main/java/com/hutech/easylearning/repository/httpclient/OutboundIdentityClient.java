package com.hutech.easylearning.repository.httpclient;



import com.hutech.easylearning.dto.reponse.ExchangeTokenResponse;
import com.hutech.easylearning.dto.request.ExchangeTokenRequest;
import feign.QueryMap;

import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.PostMapping;

@FeignClient(name = "outbound-identity", url = "https://oauth2.googleapis.com")
public interface OutboundIdentityClient {
    @PostMapping(value = "/token", produces = MediaType.APPLICATION_FORM_URLENCODED_VALUE)
    ExchangeTokenResponse exchangeToken(@QueryMap ExchangeTokenRequest request);
}


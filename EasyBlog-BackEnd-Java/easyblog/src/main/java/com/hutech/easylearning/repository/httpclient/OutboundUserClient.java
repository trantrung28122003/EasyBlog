package com.hutech.easylearning.repository.httpclient;

import com.hutech.easylearning.dto.reponse.OutboundUserResponse;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.http.MediaType;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.RequestParam;

@FeignClient(name = "outbound-user-client", url = "https://oauth2.googleapis.com")
public interface OutboundUserClient {
    @GetMapping(value = "/token", produces = MediaType.APPLICATION_FORM_URLENCODED_VALUE)
    OutboundUserResponse getUserInfo(@RequestParam("alt") String alt, @RequestParam("access_token") String access_token);
}

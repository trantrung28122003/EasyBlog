package com.hutech.easylearning.dto.request;

import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class LivestreamRequest {
    private String streamId;
    private String title;
    private String username;

    // Getters and setters
}


package com.hutech.easylearning.dto.request;


import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class BlogCreationRequest {
    String content;
}

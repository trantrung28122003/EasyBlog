package com.hutech.easylearning.dto.request;


import lombok.Builder;
import lombok.Data;

@Data
@Builder
public class VideoFrame {
    private String streamId;
    private byte[] frameData; // Dữ liệu khung hình dưới dạng byte array

    // Getters and setters
}


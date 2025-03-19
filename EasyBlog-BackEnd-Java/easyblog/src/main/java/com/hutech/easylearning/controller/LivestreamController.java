package com.hutech.easylearning.controller;


import com.hutech.easylearning.dto.request.LivestreamRequest;
import com.hutech.easylearning.dto.request.VideoFrame;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.bind.annotation.RequestMapping;

@RestController
@RequestMapping("/api/livestream")
@RequiredArgsConstructor
public class LivestreamController {

    private final SimpMessagingTemplate simpMessagingTemplate;
    @MessageMapping("/stream")
    @SendTo("/topic/liveStreams")
    public byte[] handleLivestream(byte[] videoFrame) {
        System.out.println("Received video frame with length: " + videoFrame.length);
        // Xử lý video frame (nếu cần), có thể thêm mã hóa, nén, v.v.
        return videoFrame;  // Trả về byte array của video frame cho tất cả client kết nối
    }

}

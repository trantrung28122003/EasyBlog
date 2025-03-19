package com.hutech.easylearning.controller;

import com.hutech.easylearning.dto.reponse.CommentResponse;
import com.hutech.easylearning.dto.reponse.ReplyResponse;
import com.hutech.easylearning.dto.request.ApiResponse;
import com.hutech.easylearning.dto.request.CommentRequest;
import com.hutech.easylearning.dto.request.ReplyRequest;
import com.hutech.easylearning.service.CommentService;
import com.hutech.easylearning.service.NotificationService;
import jakarta.websocket.server.PathParam;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.messaging.simp.SimpMessagingTemplate;
import org.springframework.web.bind.annotation.*;

import java.util.List;

@RestController
@RequestMapping("/comments")
@RequiredArgsConstructor
public class CommentController {

    private final CommentService commentService;
    private final NotificationService notificationService;
    private final SimpMessagingTemplate simpMessagingTemplate;

    @GetMapping("/commentsByBlog")
    public ApiResponse<List<CommentResponse>> getComments(@RequestParam String blogId) {
        return ApiResponse.<List<CommentResponse>>builder()
                .result(commentService.getCommentsByBlogId(blogId))
                .build();
    }

    @MessageMapping("/comment")
    @SendTo("/topic/comments")
    public CommentResponse handleComment(@RequestBody CommentRequest request) {
        System.out.println(request);
        CommentResponse commentResponse = commentService.addComment(request);
        return commentResponse;
    }

    @MessageMapping("/reply")
    @SendTo("/topic/replies")
    public ReplyResponse handleReply(@RequestBody ReplyRequest request) {
        ReplyResponse replyResponse = commentService.addReplyToComment(request);
        return replyResponse;
    }
}

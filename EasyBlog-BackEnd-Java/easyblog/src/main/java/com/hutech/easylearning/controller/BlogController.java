package com.hutech.easylearning.controller;


import com.hutech.easylearning.dto.reponse.BlogResponse;
import com.hutech.easylearning.dto.reponse.CommentResponse;
import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.*;
import com.hutech.easylearning.service.BlogLikeService;
import com.hutech.easylearning.service.BlogService;
import com.hutech.easylearning.service.UploaderService;
import jakarta.validation.Valid;
import lombok.RequiredArgsConstructor;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;
import java.util.Base64;

import java.util.List;

@RestController
@RequestMapping("/blog")
@RequiredArgsConstructor
public class BlogController {
    private final BlogService blogService;
    private final UploaderService uploaderService;
    private final BlogLikeService blogLikeService;

    //    @MessageMapping("/createBlog")
//    @SendTo("/topic/newBlogs")
    @PostMapping("/addBlog")
    public ApiResponse<BlogResponse> CreateBlog(@Valid BlogCreationRequest request, @RequestParam(value = "file", required = false) MultipartFile file) {
        return ApiResponse.<BlogResponse>builder()
                .result(blogService.createBlog(request, file))
                .build();
    }
    @PostMapping("/like")
    public ApiResponse<BlogResponse> likeBlog(@RequestParam String blogId) {
        return ApiResponse.<BlogResponse>builder()
                .result(blogLikeService.likeBlog(blogId))
                .build();
    }

    @PostMapping("/unLike")
    public ApiResponse<BlogResponse> unLikeBlog(@RequestParam String blogId) {
        return ApiResponse.<BlogResponse>builder()
                .result(blogLikeService.unlikeBlog(blogId))
                .build();
    }



    @GetMapping
    public ApiResponse<List<BlogResponse>> getBlogs() {
        return ApiResponse.<List<BlogResponse>>builder()
                .result(blogService.getAllBlog())
                .build();
    }
}

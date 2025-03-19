package com.hutech.easylearning.controller;


import com.hutech.easylearning.dto.reponse.NotificationResponse;
import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.ApiResponse;
import com.hutech.easylearning.dto.request.ChangePasswordRequest;
import com.hutech.easylearning.dto.request.ResetPasswordRequest;
import com.hutech.easylearning.service.BlogLikeService;
import com.hutech.easylearning.service.NotificationService;
import com.hutech.easylearning.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.util.List;

@RestController
@RequestMapping("/customer")
public class CustomerController {

    @Autowired
    private UserService userService;

    @Autowired
    private NotificationService notificationService;
    @Autowired
    private BlogLikeService blogLikeService;


    @GetMapping("/notificationByUser")
    public ApiResponse<List<NotificationResponse>> getNotification() {
        return ApiResponse.<List<NotificationResponse>>builder()
                .result(notificationService.getAllNotificationByUser())
                .build();
    }

    @PostMapping("/updateNotificationStatusIsRead")
    public ApiResponse<NotificationResponse> updateNotificationStatus(@RequestParam String notificationId) {
        return ApiResponse.<NotificationResponse>builder()
                .result(notificationService.updateStatusIsRead(notificationId))
                .build();
    }

    @PostMapping("/updateProfile")
    public ApiResponse<UserResponse> updateProfile(
            @RequestParam(value = "fullName") String fullName,  // Tham số fullName
            @RequestParam(value = "file", required = false) MultipartFile file) {
        System.out.println("Full Name from : " + fullName);  // Kiểm tra giá trị fullName
        System.out.println("Request: " + fullName);
        return ApiResponse.<UserResponse>builder()
                .result(userService.updateProfileUser(fullName, file))
                .build();
    }

    @PostMapping("/changePassword")
    public ApiResponse<String> changePassword(@RequestBody ChangePasswordRequest changePasswordRequest) {
        try{
            Boolean isSuccess = userService.changePassword(changePasswordRequest);
            if (isSuccess) {
                return ApiResponse.<String>builder()
                        .result("Thay đổi mật khẩu thành công")
                        .build();
            } else {
                return ApiResponse.<String>builder()
                        .code(400)
                        .message("Mật khẩu cũ không đúng")
                        .build();
            }
        }catch (Exception ex) {
            ex.printStackTrace();
            return ApiResponse.<String>builder()
                    .code(500)
                    .message("Đã xảy ra lỗi trong quá trình xác minh mã")
                    .build();
        }
    }

    @PostMapping("/resetPassword")
    public ApiResponse<String> resetPassword(@RequestBody ResetPasswordRequest resetPasswordRequest) {
        try{
           userService.resetPassword(resetPasswordRequest);
           return ApiResponse.<String>builder()
                        .result("Thay đổi mật khẩu thành công")
                        .build();
        }catch (Exception ex) {
            ex.printStackTrace();
            return ApiResponse.<String>builder()
                    .code(500)
                    .message("Đã xảy ra lỗi trong quá trình xác minh mã")
                    .build();
        }
    }

    @PostMapping
    public ApiResponse<String> likeBlog(@RequestParam String blogId) {
        blogLikeService.likeBlog(blogId);
        return ApiResponse.<String>builder()
                .result("like thanh cong")
                .build();
    }

    @DeleteMapping
    public ApiResponse<String> unlikeBlog( @RequestParam String blogId) {
        blogLikeService.unlikeBlog(blogId);
        return ApiResponse.<String>builder()
                .result("huy like thanh cong")
                .build();
    }

    @GetMapping()
    public ApiResponse<String> checkIfBlogLiked(@RequestParam String blogId) {

        try{
            boolean isSuccess = blogLikeService.isBlogLikedByUser(blogId);

            if (isSuccess) {
                return ApiResponse.<String>builder()
                        .code(200)
                        .build();
            } else {
                return ApiResponse.<String>builder()
                        .code(400)
                        .build();
            }
        }catch (Exception ex) {
            ex.printStackTrace();
            return ApiResponse.<String>builder()
                    .code(500)
                    .message("Đã xảy ra lỗi trong quá trình")
                    .build();
        }
    }
}

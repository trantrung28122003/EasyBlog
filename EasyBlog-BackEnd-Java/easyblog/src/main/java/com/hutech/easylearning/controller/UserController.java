package com.hutech.easylearning.controller;


import com.hutech.easylearning.dto.reponse.UserResponse;
import com.hutech.easylearning.dto.request.ApiResponse;
import com.hutech.easylearning.dto.request.UserCreationRequest;
import com.hutech.easylearning.dto.request.UserUpdateRequest;
import com.hutech.easylearning.entity.User;
import com.hutech.easylearning.service.UserService;
import jakarta.validation.Valid;
import lombok.extern.slf4j.Slf4j;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.multipart.MultipartFile;

import java.util.List;

@RestController
@RequestMapping("/users")
@Slf4j
public class UserController {
    @Autowired
    private UserService userService;

    @GetMapping
    public  ApiResponse<List<UserResponse>> getUsers() {

        var authentication = SecurityContextHolder.getContext().getAuthentication();

        log.info("UserName: {}", authentication.getName());
        authentication.getAuthorities().forEach(grantedAuthority -> log.info(grantedAuthority.getAuthority()));
        return ApiResponse.<List<UserResponse>>builder()
                .result(userService.getUsers())
                .build();
    }

    @GetMapping("/{userId}")
    public UserResponse getUserById(@PathVariable("userId") String UserId) {

        return userService.getUserById(UserId);
    }

    @GetMapping("/myInfo")
    public ApiResponse<UserResponse> getMyInfo() {

        return ApiResponse.<UserResponse>builder()
                .result(userService.getMyInfo())
                .build();
    }
    @PostMapping("/create")
    public ApiResponse<UserResponse> CreateUser(@RequestBody @Valid UserCreationRequest request,  @RequestParam(value = "file", required = false) MultipartFile file) {
        return ApiResponse.<UserResponse>builder()
                .result(userService.createUser(request, file))
                .build();
    }

    @PutMapping("/update/{userId}")
    public UserResponse UpdateUser(@PathVariable("userId") String userId, @RequestBody UserUpdateRequest request) {
        return  userService.updateUser(userId, request);
    }

    @DeleteMapping("/delete/{userId}")
    public String DeleteUser(@PathVariable("userId") String userId) {
        userService.deleteUser(userId);
        return "user has been deleted";
    }

    @PostMapping("/block/{userId}")
    public String blockUser(@PathVariable("userId") String userId) {
        userService.blockUser(userId);
        return "user has been block";
    }

    @PostMapping("/unblock/{userId}")
    public String unblockUser(@PathVariable("userId") String userId) {
        userService.unblockUser(userId);
        return "user has been unblock";
    }


}

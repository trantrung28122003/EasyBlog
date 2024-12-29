package com.hutech.easylearning.controller;


import com.hutech.easylearning.dto.request.ApiResponse;
import com.hutech.easylearning.dto.request.SendVerificationCodeRequest;
import com.hutech.easylearning.service.EmailService;
import com.hutech.easylearning.service.OtpService;
import com.hutech.easylearning.service.UserService;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

@RestController
@RequestMapping("/email")
public class EmailController {

    @Autowired
    private EmailService emailService;
    @Autowired
    private OtpService otpService;
    @Autowired
    private UserService userService;

    @PostMapping("/sendVerificationCode")
    public ApiResponse<String> sendVerificationCode(@RequestBody SendVerificationCodeRequest request) {
        try {
            String email = request.getEmail();
            if (!userService.isEmailExist(email)) {
                return ApiResponse.<String>builder()
                        .code(404)
                        .message("Email không tồn tại trong hệ thống")
                        .build();
            }
            String verificationCode = otpService.generateOtp(email);
            emailService.sendVerificationCode(email, verificationCode);
            return ApiResponse.<String>builder()
                    .result("Mã xác nhận đã được gửi qua email")
                    .build();
        } catch (Exception ex) {
            ex.printStackTrace();
            return ApiResponse.<String>builder()
                    .code(500)
                    .message("Không thể gửi email")
                    .build();
        }
    }


    @PostMapping("/verifyCode")
    public ApiResponse<String> verifyCode(@RequestBody SendVerificationCodeRequest request) {
        try {
            String email = request.getEmail();
            String verificationCode = request.getVerificationCode();
            boolean isVerified = otpService.verifyOtp(email, verificationCode);
            if (isVerified) {
                return ApiResponse.<String>builder()
                        .result("Mã xác nhận chính xác")
                        .build();
            } else {
                return ApiResponse.<String>builder()
                        .code(400)
                        .message("Mã xác nhận không chính xác hoặc đã hết hạn")
                        .build();
            }
        } catch (Exception ex) {
            ex.printStackTrace();
            return ApiResponse.<String>builder()
                    .code(500)
                    .message("Đã xảy ra lỗi trong quá trình xác minh mã")
                    .build();
        }
    }


}

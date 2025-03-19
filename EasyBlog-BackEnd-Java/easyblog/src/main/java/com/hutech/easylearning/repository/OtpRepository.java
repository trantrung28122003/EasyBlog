package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Otp;
import org.springframework.data.jpa.repository.JpaRepository;

import java.time.LocalDateTime;
import java.util.Optional;

public interface OtpRepository extends JpaRepository<Otp, String> {
    Optional<Otp> findByEmailAndOtp(String email, String otp);
    void deleteAllByExpirationTimeBefore(LocalDateTime now);
}
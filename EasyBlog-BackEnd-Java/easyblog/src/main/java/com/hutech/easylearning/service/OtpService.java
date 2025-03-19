package com.hutech.easylearning.service;



import com.hutech.easylearning.entity.Otp;
import com.hutech.easylearning.repository.OtpRepository;
import lombok.AccessLevel;
import lombok.RequiredArgsConstructor;
import lombok.experimental.FieldDefaults;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.time.LocalDateTime;
import java.util.Optional;

@Service
@RequiredArgsConstructor
@FieldDefaults(level = AccessLevel.PRIVATE)
public class OtpService {

    final OtpRepository otpRepository;

    public String generateOtp(String email) {
        String otp = generateRandomOtp();
        Otp otpRecord = new Otp();
        otpRecord.setEmail(email);
        otpRecord.setOtp(otp);
        otpRecord.setExpirationTime(LocalDateTime.now().plusMinutes(2));
        otpRepository.save(otpRecord);
        return otp;
    }

    public boolean verifyOtp(String email, String verificationCode) {
        Optional<Otp> optionalOtp = otpRepository.findByEmailAndOtp(email, verificationCode);
        if (optionalOtp.isEmpty()) {
            return false;
        }
        Otp otp = optionalOtp.get();
        if (otp != null && otp.getOtp().equals(verificationCode)) {
            if (otp.getExpirationTime().isBefore(LocalDateTime.now())) {
                otpRepository.delete(otp);
                return false;
            }
            return true;
        }
        return false;
    }

    private String generateRandomOtp() {
        return String.format("%06d", (int) (Math.random() * 1000000));
    }

    @Transactional
    @Scheduled(fixedRate = 300000)  // 300000ms = 5 phút
    public void deleteExpiredOtp() {
        LocalDateTime now = LocalDateTime.now();
        otpRepository.deleteAllByExpirationTimeBefore(LocalDateTime.now());
    }
}

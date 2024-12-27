package com.hutech.easylearning.dto.request;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.hutech.easylearning.validator.DobConstraint;
import jakarta.annotation.Nullable;
import jakarta.validation.constraints.Size;
import lombok.Builder;
import lombok.Data;

import java.time.LocalDate;

@Data
@Builder
public class UserCreationRequest {
    private String userName;
    private String email;
    private String fullName;
    @JsonFormat(shape = JsonFormat.Shape.STRING, pattern = "yyyy-MM-dd")
    @DobConstraint(min = 18, message = "INVALID_DOB")
    @Nullable
    private LocalDate dayOfBirth;
    private String imageUrl;
    @Size(min = 8, message ="pass word must be at least 8 characters")
    private String password;
}

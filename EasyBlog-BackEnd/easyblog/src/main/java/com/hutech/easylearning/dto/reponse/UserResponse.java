package com.hutech.easylearning.dto.reponse;


import com.hutech.easylearning.entity.Role;
import jakarta.persistence.Column;
import jakarta.persistence.GeneratedValue;
import jakarta.persistence.GenerationType;
import jakarta.persistence.Id;
import lombok.*;
import lombok.experimental.FieldDefaults;
import lombok.experimental.FieldNameConstants;

import java.time.LocalDate;
import java.time.LocalDateTime;
import java.util.Set;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class UserResponse {
    String id;
    String userName;
    String email;
    String fullName;
    LocalDate dayOfBirth;
    String imageUrl;
    Set<RoleResponse> roles;
    LocalDateTime dateCreate;
    LocalDateTime dateChange;
    String changedBy;
    boolean isDeleted;
}

package com.hutech.easylearning.dto.reponse;


import com.hutech.easylearning.enums.NotificationType;
import jakarta.persistence.Column;
import jakarta.persistence.EnumType;
import jakarta.persistence.Enumerated;
import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;

@Data
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class NotificationResponse {
    String id;
    String contentNotification;
    LocalDateTime dateCreate;
    Boolean isRead;
    @Enumerated(EnumType.STRING)
    NotificationType type;
    String targetId;

}

package com.hutech.easylearning.dto.reponse;

import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class ReplyResponse {
    String id;
    String commentId;
    String contentReply;
    String userId;
    String userFullName;
    String userImageUrl;
    LocalDateTime dateCreate;
}

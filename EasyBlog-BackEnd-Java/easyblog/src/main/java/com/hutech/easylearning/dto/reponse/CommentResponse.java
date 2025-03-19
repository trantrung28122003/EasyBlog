package com.hutech.easylearning.dto.reponse;


import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;
import java.util.List;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class CommentResponse {
    String id;
    String contentComment;
    String userId;
    String userFullName;
    String userImageUrl;
    LocalDateTime dateCreate;
    List<ReplyResponse> replies;
}

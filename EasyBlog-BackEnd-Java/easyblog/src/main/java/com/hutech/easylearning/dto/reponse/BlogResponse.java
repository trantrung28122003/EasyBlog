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
public class BlogResponse {
    String id;
    String content;
    String imageUrl;
    LocalDateTime dateCreate;
    int likeCount;
    int commentCount;
    String userFullName;
    String userAvatarUrl;
    boolean statusLikeByUser;
}

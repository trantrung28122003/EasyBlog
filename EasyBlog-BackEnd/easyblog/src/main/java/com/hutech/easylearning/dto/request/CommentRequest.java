package com.hutech.easylearning.dto.request;


import lombok.AccessLevel;
import lombok.Builder;
import lombok.Data;
import lombok.experimental.FieldDefaults;

@Data
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class CommentRequest {
    String trainingPartId;
    String commentContent;
    String userId;
}

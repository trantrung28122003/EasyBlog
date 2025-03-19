package com.hutech.easylearning.dto.reponse;


import lombok.*;
import lombok.experimental.FieldDefaults;

import java.math.BigDecimal;

@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class UserNoteResponse {
    String id;
    String noteContent;
    BigDecimal timeStamp;
    String courseId;
    String trainingPartId;
    String trainingPartName;
    String courseEventName;
}

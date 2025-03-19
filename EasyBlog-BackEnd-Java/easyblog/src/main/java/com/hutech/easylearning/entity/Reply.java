package com.hutech.easylearning.entity;


import com.fasterxml.jackson.annotation.JsonBackReference;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;

@Entity
@Table(name = "reply")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class Reply {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name ="id", columnDefinition = "VARCHAR(36)")
    String id;

    @Column(name = "content", nullable = false)
    String content;

    @Column(name = "user_id", nullable = false)
    String userId;

    @Column(name = "comment_id")
    String commentId;
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "comment_id", referencedColumnName = "id" , insertable = false, updatable = false)
    @JsonBackReference
    Comment comment;

    @Column(name = "date_create", nullable = false)
    LocalDateTime dateCreate;

    @Column(name = "date_change")
    LocalDateTime dateChange;

    @Column(name = "changed_by")
    String changedBy;

    @Column(name = "is_deleted", nullable = false)
    boolean isDeleted;

}

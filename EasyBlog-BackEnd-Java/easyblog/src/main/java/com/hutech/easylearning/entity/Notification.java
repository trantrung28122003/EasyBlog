package com.hutech.easylearning.entity;


import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import com.hutech.easylearning.enums.NotificationType;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;
import java.util.Set;

@Entity
@Table(name = "notification")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class Notification {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name ="id", columnDefinition = "VARCHAR(36)")
    String id;

    @Column(name = "content_notification")
    private String content;

    @Column(name = "is_read", nullable = false)
    private boolean isRead = false;

    @Column(name = "type", nullable = false)
    @Enumerated(EnumType.STRING)
    NotificationType type;

    @Column(name = "target_id", nullable = true)
    String targetId;

    @Column(name = "user_id")
    String userId;
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", referencedColumnName = "id", insertable = false, updatable = false)
    @JsonBackReference
    User user;

    @Column(name = "date_create")
    LocalDateTime dateCreate;

    @Column(name = "date_change")
    LocalDateTime dateChange;

    @Column(name = "changed_by")
    String changedBy;

    @Column(name = "is_deleted")
    boolean isDeleted;
}

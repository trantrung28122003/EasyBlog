package com.hutech.easylearning.entity;

import com.fasterxml.jackson.annotation.JsonBackReference;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;

@Entity
@Table(name = "blog_like")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class BlogLike {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name ="id", columnDefinition = "VARCHAR(36)")
    String id;

    @Column(name = "blog_id")
    String blogId;
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "blog_id", referencedColumnName = "id", insertable = false, updatable = false)
    @JsonBackReference
    Blog blog;

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


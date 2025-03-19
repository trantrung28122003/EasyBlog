package com.hutech.easylearning.entity;

import com.fasterxml.jackson.annotation.JsonBackReference;
import com.fasterxml.jackson.annotation.JsonManagedReference;
import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;
import java.util.Set;


@Entity
@Table(name = "comment")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class Comment {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name ="id", columnDefinition = "VARCHAR(36)")
    String id;

    @Column(name = "content", nullable = false)
    String content;

    @Column(name = "user_id", nullable = false)
    String userId;

    @OneToMany(mappedBy = "comment", cascade = CascadeType.ALL, fetch = FetchType.LAZY)
    @JsonManagedReference
    Set<Reply> replies;

    @Column(name = "date_create")
    LocalDateTime dateCreate;

    @Column(name = "date_change")
    LocalDateTime dateChange;

    @Column(name = "changed_by")
    String changedBy;

    @Column(name = "is_deleted")
    boolean isDeleted;

    @Column(name = "blog_id")
    String blogId;
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "blog_id", referencedColumnName = "id", insertable = false, updatable = false)
    @JsonBackReference
    Blog blog;
}

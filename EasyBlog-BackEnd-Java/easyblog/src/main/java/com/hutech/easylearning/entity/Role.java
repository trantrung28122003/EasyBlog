package com.hutech.easylearning.entity;


import jakarta.persistence.*;
import lombok.*;
import lombok.experimental.FieldDefaults;

import java.time.LocalDateTime;
import java.util.List;
import java.util.Set;

@Entity
@Table(name = "role")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@FieldDefaults(level = AccessLevel.PRIVATE)
public class Role {
    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name ="id", columnDefinition = "VARCHAR(36)")
    String id;

    @Column(name = "name")
    String name;

    @Column(name = "description")
    String description;

    @Column(name = "dateCreate")
    LocalDateTime dateCreate;

    @Column(name = "dateChange")
    LocalDateTime dateChange;

    @Column(name = "changedBy")
    String changedBy;

    @Column(name = "isDeleted")
    boolean isDeleted;

    @ManyToMany(fetch = FetchType.EAGER)
    @JoinTable(name = "user_role",
            joinColumns = @JoinColumn(name = "role_id"),
            inverseJoinColumns =  @JoinColumn(name = "user_id")
    )
    private Set<User> users;
}

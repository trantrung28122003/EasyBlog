package com.hutech.easylearning.entity;

import jakarta.persistence.*;
import lombok.*;
import java.time.LocalDateTime;

@Entity
@Table(name = "otp")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
public class Otp {

    @Id
    @GeneratedValue(strategy = GenerationType.UUID)
    @Column(name ="id", columnDefinition = "VARCHAR(36)")
    String id;

    @Column(nullable = false)
    private String email;

    @Column(nullable = false)
    private String otp;

    @Column(nullable = false)
    private LocalDateTime expirationTime;
}

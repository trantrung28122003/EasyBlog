package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface UserRepository  extends JpaRepository<User, String> {
    boolean existsByUserName(String userName);
    Optional<User> findByUserName(String userName);
    Optional<User> findByEmail(String email);
    boolean existsByEmail(String email);
}

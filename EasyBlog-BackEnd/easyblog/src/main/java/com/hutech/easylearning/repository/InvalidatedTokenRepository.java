package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.InvalidatedToken;
import com.hutech.easylearning.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface InvalidatedTokenRepository extends JpaRepository<InvalidatedToken, String> {
}

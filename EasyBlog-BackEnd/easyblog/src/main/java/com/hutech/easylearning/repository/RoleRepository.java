package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Role;
import com.hutech.easylearning.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
import java.util.Optional;

@Repository
public interface RoleRepository extends JpaRepository<Role, String> {
    List<Role> findByNameIn(List<String> names);
    Optional<Role> findByName(String name);
}

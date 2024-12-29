package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Blog;
import com.hutech.easylearning.entity.InvalidatedToken;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface BlogRepository extends JpaRepository<Blog, String> {
}

package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Comment;
import com.hutech.easylearning.entity.User;
import org.springframework.data.jpa.repository.JpaRepository;

import java.util.List;

public interface CommentRepository extends JpaRepository<Comment, String> {
    List<Comment> findAllByBlogId(String blogId);
}

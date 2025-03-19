package com.hutech.easylearning.repository;

import com.hutech.easylearning.entity.Blog;
import com.hutech.easylearning.entity.BlogLike;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BlogLikeRepository  extends JpaRepository<BlogLike, String> {
    boolean existsByBlogIdAndUserId(String blogId, String userId);
    void deleteByBlogIdAndUserId(String blogId, String userId);
    int countByBlogId(String blogId);
}

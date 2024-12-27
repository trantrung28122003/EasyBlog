package com.hutech.easylearning.exception;

import lombok.Getter;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;

@Getter
public enum ErrorCode {
    UNCATEGORIZED_EXCEPTION(999, "Uncategorized error", HttpStatus.INTERNAL_SERVER_ERROR),
    USER_EXISTED(409, "User existed", HttpStatus.BAD_REQUEST),
    USER_NOT_EXISTED(404, "User not found", HttpStatus.NOT_FOUND),
    UNAUTHENTICATED(401, "Unauthorized: Incorrect username or password", HttpStatus.UNAUTHORIZED),
    UNAUTHORIZED(403  , "UNAUTHORIZED:You do not permission", HttpStatus.FORBIDDEN),
    INVALID_DOB(403, "Your age must be at least {min}", HttpStatus.BAD_REQUEST),
    UPDATE_OWN_FEEDBACK_ONLY(403, "You can only update your own feedback", HttpStatus.FORBIDDEN),
    USER_BLOCKED(403, "User is blocked", HttpStatus.FORBIDDEN);

    ErrorCode(int code, String message, HttpStatusCode statusCode) {
        this.code = code;
        this.message = message;
        this.statusCode = statusCode;
    }

    private int code;
    private String message;
    private HttpStatusCode statusCode;
}


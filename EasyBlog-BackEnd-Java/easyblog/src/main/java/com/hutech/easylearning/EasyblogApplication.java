package com.hutech.easylearning;

import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.cloud.openfeign.EnableFeignClients;

@SpringBootApplication
@EnableFeignClients
public class EasyblogApplication {
	public static void main(String[] args) {
		SpringApplication.run(EasyblogApplication.class, args);
	}

}


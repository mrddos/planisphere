package org.healer.service;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

public interface RESTService {

	void doService(HttpServletRequest request, HttpServletResponse response);
	
	/* REST Service */
	void onGet(HttpServletRequest request, HttpServletResponse response);
	void onPost(HttpServletRequest request, HttpServletResponse response);
	void onPut(HttpServletRequest request, HttpServletResponse response);
	void onDelete(HttpServletRequest request, HttpServletResponse response);
}

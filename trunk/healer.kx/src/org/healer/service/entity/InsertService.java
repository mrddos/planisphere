package org.healer.service.entity;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.service.RESTService;

public class InsertService implements RESTService {


	private static final String JDO_PACKAGE_PREFIX = "org.healer.jpa.";
	
	
	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		


	}
	
	/**
	 * POST service/entity/insert?name=File
	 */
	@Override
	public void onPost(HttpServletRequest request, HttpServletResponse response) {

		
	}

	@Override
	public void onPut(HttpServletRequest request, HttpServletResponse response) {

	}

	@Override
	public void onDelete(HttpServletRequest request,
			HttpServletResponse response) {

	}

	@Override
	public void doService(HttpServletRequest request,
			HttpServletResponse response) {
		// TODO Auto-generated method stub
		
	}

}

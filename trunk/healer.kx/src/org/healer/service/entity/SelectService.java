package org.healer.service.entity;

import java.util.List;

import javax.jdo.Query;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


import org.healer.service.RESTService;

public class SelectService implements RESTService {
	
	
	private static final String JDO_PACKAGE_PREFIX = "org.healer.jdo.";

	/**
	 * service/entity/select?name=File
	 */
	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		
		/*
		String name = request.getParameter("name");
		String className = JDO_PACKAGE_PREFIX + name;
		try {
			Class<?> clz = Class.forName(className);
			
			Query query = PMF.get(null, null).newQuery(clz);
			
			@SuppressWarnings("unchecked")
			List<Item> result = (List<Item>)query.execute();
			
			if (result != null) {
				for (Item item: result) {
					System.out.print(item);
					System.out.print(item.getClass());
				}
			} else {
				System.out.println(1);
			}
		} catch (ClassNotFoundException e) {
			e.printStackTrace();
		}
		*/

	}

	@Override
	public void onPost(HttpServletRequest request, HttpServletResponse response) {
		// TODO Auto-generated method stub

	}

	@Override
	public void onPut(HttpServletRequest request, HttpServletResponse response) {
		// TODO Auto-generated method stub

	}

	@Override
	public void onDelete(HttpServletRequest request,
			HttpServletResponse response) {
		// TODO Auto-generated method stub

	}

	@Override
	public void doService(HttpServletRequest request,
			HttpServletResponse response) {
		// TODO Auto-generated method stub
		
	}

}

package org.healer.service.entity;

import java.util.List;

import javax.jdo.PersistenceManager;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;


import org.healer.jpa.Account;
import org.healer.jpa.AccountManager;
import org.healer.service.RESTService;

public class InsertService implements RESTService {


	private static final String JDO_PACKAGE_PREFIX = "org.healer.jpa.";
	
	
	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		

		Account oneAccount = AccountManager.addAccount("healer_kx@163.com");
		
		
		List<Account> accounts = AccountManager.listAccounts();
		for (Account account: accounts) {
			System.out.println(account);
		}
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

}

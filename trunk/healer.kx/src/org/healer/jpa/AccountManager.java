package org.healer.jpa;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.EntityTransaction;
import javax.persistence.Query;

public class AccountManager {

	
	
	
	
	
	public static Account addAccount(String userName) {
		
		
		EntityManager em = EMF.get().createEntityManager();
		Account account = new Account(userName);
		EntityTransaction et = em.getTransaction();
		et.begin();
		em.persist(account);
		et.commit();
		
		em.close();
		return account;
		
	}
	
	@SuppressWarnings("unchecked")
	public static List<Account> listAccounts() {
	
		List<Account> accounts = null;
		
		EntityManager em = EMF.get().createEntityManager();

		EntityTransaction et = em.getTransaction();
		et.begin();


		Query query = em.createQuery("SELECT a FROM Account a");
		accounts = (List<Account>)query.getResultList();
		
		et.commit();
		
		em.close();
		return accounts;
	}
	
	public static boolean removeAccount(String userName) {
		
		return false;
	}
}

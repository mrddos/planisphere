package org.healer.jpa;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.EntityTransaction;
import javax.persistence.Query;

public class TagManager {
	
	public static Long addTag(String itemId, String itemType, String name) {
		
		
		EntityManager em = EMF.get().createEntityManager();
		Tag tag = new Tag(itemId, itemType, name);
		EntityTransaction et = em.getTransaction();
		et.begin();
		em.persist(tag);
		et.commit();
		
		em.close();
		return (long) 0;
		
	}
	
	@SuppressWarnings("unchecked")
	public static List<Tag> listAccounts() {
	
		List<Tag> tags = null;
		
		EntityManager em = EMF.get().createEntityManager();

		EntityTransaction et = em.getTransaction();
		et.begin();


		Query query = em.createQuery("SELECT t FROM Tag t");
		tags = (List<Tag>)query.getResultList();
		
		et.commit();
		
		em.close();
		return tags;
	}
	
	public static boolean removeAccount(String userName) {
		
		return false;
	}

}

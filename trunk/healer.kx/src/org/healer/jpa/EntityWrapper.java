package org.healer.jpa;

import java.lang.ref.WeakReference;

import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.EntityTransaction;

public class EntityWrapper {

	
	
	public static <E> E add(E entity) {
		EntityManagerFactory emf = EMF.get();
		if (emf != null) {
			EntityManager em = emf.createEntityManager();
			if (em != null) {
				EntityTransaction et = em.getTransaction();
				et.begin();
				em.persist(entity);
				et.commit();
				
				em.close();
				return entity;
			}
		}
		return null;
	}
	
	public static void testAdd(Object... params) {
		

		Tag tag = EntityWrapper.<Tag>add(new Tag());
		
		
	}
	
}

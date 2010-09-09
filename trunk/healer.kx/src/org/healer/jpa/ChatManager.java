package org.healer.jpa;

import java.util.List;

import javax.persistence.EntityManager;
import javax.persistence.EntityTransaction;
import javax.persistence.Query;

public class ChatManager {

	
	public static ChatMessage addMessage(String message) {
		EntityManager em = EMF.get().createEntityManager();
		ChatMessage cm = new ChatMessage(message);
		EntityTransaction et = em.getTransaction();
		et.begin();
		em.persist(cm);
		et.commit();
		
		em.close();
		return cm;
	}
	
	
	@SuppressWarnings("unchecked")
	public static ChatMessage latestMessage() {
	
		List<ChatMessage> messages = null;
		
		EntityManager em = EMF.get().createEntityManager();

		EntityTransaction et = em.getTransaction();
		et.begin();


		Query query = em.createQuery("SELECT cm FROM ChatMessage cm");
		messages = (List<ChatMessage>)query.getResultList();
		
		et.commit();
		
		em.close();
		if (messages.size() == 0) {
			return new ChatMessage("");
		}
		return messages.get(messages.size() - 1);
	}
}

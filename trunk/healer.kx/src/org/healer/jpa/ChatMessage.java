package org.healer.jpa;

import javax.jdo.annotations.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;

@Entity
@Table(name = "ChatMessage")
public class ChatMessage {
	
	private Long id;
	
	private String content;
	

	
	
	protected ChatMessage() {

	}
	
	public ChatMessage(String content) {
		this.content = content;
	}
	


	@Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
	public Long getId() {
		return id;
	}

	public void setId(Long id) {
		this.id = id;
	}

	public void setContent(String content) {
		this.content = content;
	}

	@Column(length = 512)
	public String getContent() {
		return content;
	}

}

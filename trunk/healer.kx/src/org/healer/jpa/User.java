package org.healer.jpa;

import javax.jdo.annotations.Column;
import javax.persistence.Entity;
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;
import javax.persistence.Table;


@Entity
@Table(name = "User")
public class User {
	
	
	private Long id;
	
	private String userEmail;
	
	
	protected User() {

	}
	
	public User(String userEmail) {
		this.userEmail = userEmail;
	}
	


	@Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
	public Long getId() {
		return id;
	}

	public void setId(Long id) {
		this.id = id;
	}



	@Column
	public String getUserEmail() {
		return userEmail;
	}

	public void setUserEmail(String userEmail) {
		this.userEmail = userEmail;
	}

	
	public String toString() {
		return String.format("# %s [ID: %d]", userEmail, id.longValue());
	}

}

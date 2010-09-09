package org.healer.service.chat;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.jpa.ChatManager;
import org.healer.service.RESTService;

public class SendService implements RESTService {

	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void onPost(HttpServletRequest request, HttpServletResponse response) {
		String message = request.getParameter("msg");
		System.out.println(message);
		
		ChatManager.addMessage(message);
		
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

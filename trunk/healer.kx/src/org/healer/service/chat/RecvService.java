package org.healer.service.chat;

import java.io.IOException;
import java.io.Writer;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;

import org.healer.jpa.ChatManager;
import org.healer.jpa.ChatMessage;
import org.healer.service.RESTService;

public class RecvService implements RESTService {

	@Override
	public void onGet(HttpServletRequest request, HttpServletResponse response) {
		ChatMessage cm = ChatManager.latestMessage();
		if (cm != null) {
			String message = cm.getContent();
			System.out.println(cm.getContent());
			try {
				response.setContentType("text/plain");
				response.setCharacterEncoding("utf-8");
				Writer writer = response.getWriter();
				writer.write(message);
				writer.flush();
				writer.close();
			} catch (IOException e) {
				e.printStackTrace();
			}
		} else {
			System.out.println("Error");
			
		}
		
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

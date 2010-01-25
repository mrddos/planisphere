package org.healer;

import java.io.IOException;
import javax.servlet.http.*;

@SuppressWarnings("serial")
public class WelcomeHealerServlet extends HttpServlet {
	public void doGet(HttpServletRequest req, HttpServletResponse resp)
			throws IOException {
		
		
		String content = "";

		
		resp.getWriter().println(content);
		

		
	}
}

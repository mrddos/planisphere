package org.healer;

import java.io.IOException;
import javax.servlet.http.*;

@SuppressWarnings("serial")
public class WelcomeHealerServlet extends HttpServlet {
	public void doGet(HttpServletRequest req, HttpServletResponse resp)
			throws IOException {
		resp.setContentType("text/plain");
		resp.getWriter().println("I love Yueyue~");
		

		
	}
}

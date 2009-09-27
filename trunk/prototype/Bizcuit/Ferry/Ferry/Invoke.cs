using System;
using System.Collections.Generic;
using System.Text;
using Ferry.Notification;

namespace Ferry
{

    public delegate object Invoke(Delegate method, params object[] args);



	internal delegate object InvokeProxy(NotifySet.Notification n, object[] objects);

	internal delegate object InvokeAllProxy(List<NotifySet.Notification> notifyList, object[] objects);




}

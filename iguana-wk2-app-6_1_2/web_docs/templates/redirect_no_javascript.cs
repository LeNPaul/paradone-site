      <!-- No Javascript -->
      <noscript>

         <div id="header">
            <div id="logo"><img src="/images/iguana_logo.png"/></div>
         </div>

         <div id="main">

            <table id="iguana">
               <tr>
                  <td id="cookie_crumb">
                     No Javascript
                  </td>
               </tr>

               <tr><td id="dashboard_body" style="height: 0px;"><center>

                     <h3><font color="red">Iguana requires Javascript to be enabled on your browser.</font></h3>
                     <p>
                        When you have enabled Javascript, <a href="javascript:document.redirect.submit();">continue</a>.
                        <?cs if:IncludeLogoutButton ?>  Otherwise, logout below.<?cs /if ?>
                     </p>

                  </center></td>
               </tr>

               <?cs if:IncludeLogoutButton ?>
                  <tr><td columnspan="2" align="center">
                     <table id="buttons"><tr><td>
                        <a class="action-button blue" href="/login.html"><span>Logout</span></a>
                     </td></tr></table>
                  </td></tr>
               <?cs /if ?>

            </table>

         </div>
    
      </noscript>

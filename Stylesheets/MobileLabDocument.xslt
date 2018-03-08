<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
<xsl:preserve-space elements="Notes"/>
<xsl:preserve-space elements="ObservationIdentifier"/>


	<xsl:template match="/">
		<HTML>
			<HEAD>
        <meta name='format-detection' content='telephone=no'/>
        <style type="text/css" media="screen">
          br
          {
          line-height:3px;
          }
          .captionFont
          {
          font-family:tahoma;
          font-weight:bold;
          font-size:12px;
          text-align: eft;
          vertical-align:top;
          color: #707c88;
          }
          .Header
          {
          font-family:tahoma;
          font-weight:bold;
          font-size:12px;
          vertical-align:top;
          color: #000000;
          }
          .TestNameHeader
          {
          font-family:tahoma;
          font-weight:bold;
          font-size:12px;
          vertical-align:top;
          color: #000000;
          Padding-Left:2px;
          }
          .CompanyHeader
          {
          font-family:tahoma;
          font-weight:bold;
          font-size:14px;
          vertical-align:top;
          color: #000000;
          }
          .Label
          {
          font-family:tahoma;
          font-weight:bold;
          font-size:11px;
          text-align:left;
          }
          .DataText
          {
          font-family:tahoma;
          font-size:11px;
          text-align:left;
          }
          .ResultText
          {
          font-family:tahoma;
          font-size:12px;
          }
          .AbnormalResultText
          {
          font-family:tahoma;
          font-size:12px;
          font-weight:bold;
          color:red;
          }
          .NoteText
          {
          font-Size:11px;
          font-Family:Lucida Console;
          color:#666666;
          padding-left:30px;
          }
          .LabGeneratedNoteText
          {
          font-Size:11px;
          font-Family:Lucida Console;
          color:#666666;
          padding-left:40px;
          }
          .subTestNameHeader
          {
          font-family:tahoma;
          font-size:12px;
          vertical-align:top;
          color: #000000;
          padding-left:10px;
          font-weight:bold;
          }
        </style>
			</HEAD>
			<BODY  bgcolor="transparent" marginheight="0" marginwidth="0" rightmargin="0" leftmargin="0" topmargin="0" bottommargin="0" >
				<div align="left" style="padding-top:3px;padding-left:30px;">
					<table>
						<tr >
							<td  colspan='2'>
								<span id="labtype" Class="CompanyHeader">
									<xsl:value-of select="//labheader/@labcompanyname" />
									
									<xsl:choose>
										<xsl:when test='(//labheader/@doctype) = "MICRO" '>
											<xsl:text> - Mirobiology</xsl:text>
										</xsl:when>
										<xsl:when test='(//labheader/@doctype) = "TRANSCRIPTION" '>
											<xsl:text> - Transcription</xsl:text>
										</xsl:when>
										<xsl:when test='(//labheader/@doctype) = "LAB" '>
											<xsl:text> - Lab</xsl:text>
										</xsl:when>
										<xsl:when test='(//labheader/@doctype) = "PATHOLOGY" '>
											<xsl:text> - Pathology</xsl:text>
										</xsl:when>
										<xsl:when test='(//labheader/@doctype) = "Radiology" '>
											<xsl:text> - Radiology</xsl:text>
										</xsl:when>
										<xsl:otherwise >
											<xsl:choose>
												<xsl:when test='(//labheader/@doctype) != "" '>
													<xsl:text> - </xsl:text>
														<xsl:value-of select="//labheader/@doctype" />
												</xsl:when>
											</xsl:choose>
										</xsl:otherwise>
									</xsl:choose>

									<xsl:choose>
										<xsl:when test='(//labheader/@subdoctype) != "" and (//labheader/@subdoctype) != "None"'>
											<xsl:text> - </xsl:text>
											<xsl:value-of select="//labheader/@subdoctype" />
										</xsl:when>
									</xsl:choose>
								</span>
							</td>
						</tr>
						<tr>
							<td width='75px' >
								<span class='Label'>Specimen #: </span>
							</td>
							<td style="padding-right:30px;">
								<span  class='DataText' >
									<xsl:value-of  select='//specimen/@specimennumber'/>		
								</span>								
							</td>							
							<td  width='100px'>
								<span class='Label' >Report Status: </span>
							</td>
							<td style="padding-right:50px;">
								<span class='DataText' >
									<xsl:choose>
										<xsl:when test='count(.//panel/subpanel) > 0'>
											<xsl:value-of select='//order/panel/subpanel/@reportstatus'/>
										</xsl:when>
										<xsl:otherwise >
											<xsl:value-of select='//order/panel/@reportstatus'/>
										</xsl:otherwise>
									</xsl:choose>
								</span>
							</td>
						</tr>
					</table>
					<br/>
					<table>
						<tr>
							<td  colspan='2'>
								<span Class="Header">Patient Information: </span>
							</td>
						</tr>
						<tr>
							<td  width='75px'>
								<span class='Label' >Name: </span>
							</td>
							<td style="padding-right:50px;">
								<span id='patientname' class='DataText' >
									<xsl:apply-templates select="/labresult/patient" />
								</span>
							</td>
  							<td >
								<span  class='Label'>PatientID: </span>
							</td>
							<td  style="padding-right:50px;">
								<span id='patientid' class='DataText' >
									<xsl:value-of select="/labresult/patient/@primesuiteid" />
								</span>
							</td>
							<td >
								<span  class='Label' >Sex: </span>
							</td>
							<td  style="padding-right:50px;">
								<span class='DataText' >
									<xsl:value-of select="/labresult/patient/@sex" />
								</span>
							</td>
							<td >
								<span class='Label'>DOB: </span>
							</td>
							<td>
								<span class='DataText' >
									<xsl:value-of select="/labresult/patient/@dob" />
								</span>
								
							</td>
						</tr>
						<tr>
							<td  width='75px' valign='top'>
								<span class='Label'>Address: </span>
							</td>
							<td style="padding-right:50px;">
								<span class='DataText' >
									<xsl:apply-templates select="/labresult/patient/address" />
								</span>
								
							</td>
							<td valign='top'>
								<span class='label'>Phone #: </span>
							</td>
							<td   valign='top' style="padding-right:50px;">
								<span class='DataText' >
									<xsl:value-of select="/labresult/patient/@phone" />
								</span>
							</td>
							<td valign='top'>
								<span class='label'>SSN: </span>
							</td>
							<td   valign='top' style="padding-right:50px;">
								<span class='DataText' >
									<xsl:value-of select="/labresult/patient/@ssn" />
								</span>
							</td>
						</tr>
					</table>
					
					<br/>
						
					<xsl:apply-templates mode="header" select="/labresult/patient/order[1]" />
					<xsl:apply-templates mode="labbody" select="/labresult/patient/order" />
					<xsl:apply-templates mode="labnote" select="/labresult/patient/order" />
		<hr style="width:700px;"/>
					<xsl:apply-templates mode="footer" select="/labresult" />
		<hr style="width:700px;"/>
				</div>
			</BODY>
		</HTML>
	</xsl:template>
	
	
	<xsl:template match="patient">
		<xsl:value-of select="@lastname" />
		<xsl:if test="@lastname != ''">
			<xsl:text>, </xsl:text>
		</xsl:if>
		<xsl:value-of select="@firstname" /> 
		<xsl:text> </xsl:text>
		<xsl:value-of select="@middlename" />
		<xsl:if test="@middlename != ''">
			<xsl:text> </xsl:text>
		</xsl:if>
		<xsl:value-of select="@suffix" />
	</xsl:template>
	
	
	<xsl:template match="orderingprovider">
		<xsl:value-of select="@lastname" />
		<xsl:if test="@lastname != ''">
			<xsl:text>, </xsl:text>
		</xsl:if>
		<xsl:value-of select="@firstname" /> 
		<xsl:text> </xsl:text>
		<xsl:value-of select="@middlename" />
		<xsl:if test="@middlename != ''">
			<xsl:text> </xsl:text>
		</xsl:if>
		<xsl:value-of select="@suffix" />
		
		<xsl:if test="@suffix != ''">
			<xsl:text> </xsl:text>
		</xsl:if>
		
		<xsl:if test="@degree != ''">
			<xsl:text>,</xsl:text>
		</xsl:if>
		<xsl:value-of select="@degree" />
	</xsl:template>
	
	
	
	<xsl:template match="order" mode="header">	
		<table  cellpadding="0" cellspacing="0">
			<tr>
				<td valign='top'>
					<table border='0'>
						<tr>
							
							<td  >
								<span  class='Label'>Date of Specimen: </span>
							</td>
						
							<td style="padding-right:20px;">
								<span  class='DataText' >
									<xsl:value-of  select='./specimen/@datetaken'/>
								</span>										
							</td>
						
							<td  >
								<span  class='Label' >Date Received: </span>
							</td>
						
							<td >
								<span  class='DataText' >
									<xsl:value-of  select='./specimen/@datereceived'/>
								</span>										
							</td>
							
						</tr>
						<tr>
							<td  >
								<span  class='Label' >Date Reported: </span>
							</td>
							<td  >
								<span  class='DataText' >
									<xsl:value-of select='./specimen/@datereported'/>
								</span>
							</td>
							<td  >
								<span  class='Label' >Physician: </span>
							</td>
							<td>
								<span  class='DataText' >
									<xsl:apply-templates  select='./orderingprovider'/>
								</span>
							</td>
							<td  >
								<span  class='Label' >Information: </span>
							</td>
							<td>
								<span  class='DataText' >
									<xsl:value-of select='./specimen/@RelevantClinicalInformation'/>
								</span>
							</td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
					
		<br/>
		
		<div style="display:block; padding-Bottom:7px" align="left">
			<hr style="width:700px;"/>
			<table cellpadding='0' cellspacing='0' style='table-layout:fixed'>
				<colgroup>
					<col align='left' width='100px' />
				</colgroup>
				<tbody>
					<tr>
						<td class='captionFont'>Test Name</td>
					</tr>
				</tbody>
			</table>
			
			<hr style="width:700px;"/>
			
			<xsl:if test='count(/labresults/patient/notes) > 0'>
				<Table>
					<TR>
						<td valign='top'>
							<span  class='Label' >Comments: </span>
						</td>
						<td>
							<span  class='DataText' >
								<xsl:apply-templates   select='/labresult/patient/notes'/>
							</span>
						</td>
					</TR>
				</Table>
				<br/>
			</xsl:if>
		</div>		
	</xsl:template>


		<xsl:template mode="footer" match="labcompany">
		<table border='0'>
			<tr>
				<td width='75px' >
					<span class='Label'>Lab Name: </span>
					
				</td>
				<td style="padding-right:30px;">
					<span  class='DataText' >
						<xsl:value-of select="@name"/>	
						<xsl:if test="@id != ''">
							<xsl:text> (</xsl:text>
							<xsl:value-of select="@id" /> 
							<xsl:text>)</xsl:text>
						</xsl:if>
					</span>								
				</td>
				<td width='50px' >
					<span class='Label'>Director: </span>
					
				</td>
				<td >
					<span  class='DataText' >
						<xsl:value-of select="@director" />
					</span>								
				</td>
			</tr>
			<tr>
				<td valign='top' width='75px' >
					<span class='Label'>Address: </span>
				</td>
				<td valign='top' style="padding-right:30px;">
					<span  class='DataText' >
						<xsl:apply-templates select="./address" />	
					</span>								
				</td>
				<td valign='top' width='60px' >
					<span class='Label'>Phone #: </span>
				</td>
				<td valign='top' style="padding-right:30px;">
					<span  class='DataText' >
						<xsl:value-of select="@phone" />
					</span>								
				</td>
			</tr>
		</table>
		<br/>		
	</xsl:template>

	
<xsl:template mode="labbody" match="order">

		<xsl:for-each select="./*">
			<xsl:choose>
				<xsl:when test='name() = "panel"'>
					<span class="TestNameHeader" >
						<xsl:value-of select="@name" />
					</span>
					<xsl:apply-templates select="./notes" />	
						
					<xsl:if test='count(./subpanel) > 0'>
						<xsl:apply-templates select="./subpanel" />	
					</xsl:if>
					<xsl:if test='count(./result) > 0'>
						<xsl:apply-templates select="./result" />
					</xsl:if>	
					
					
											
					<xsl:if test='name(following-sibling::*[position()=1]) = "specimen"'>
						<br/>
						<xsl:apply-templates select="preceding-sibling::specimen[last()]/notes"/>
						<br/><br/><br/>
					</xsl:if>
					
					<xsl:if test='count(following-sibling::specimen) = 0'>
						<br/>
						<xsl:apply-templates select="preceding-sibling::specimen[last()]/notes"/>
						<br/><br/><br/>
					</xsl:if>
				</xsl:when>
			</xsl:choose>
		</xsl:for-each>
	</xsl:template>
	
	
	
	
	
	<xsl:template mode="labnote" match="order/specimen/notes/line">
		<span class='DataText' style='width:700px; padding-left:10px;'>
			<xsl:value-of select='@value' disable-output-escaping="yes"/>
		    <br/>
		</span>
		<br/>
	</xsl:template>
	
	
	<xsl:template  match="subpanel">
		<div align="left" style='padding-left:10px;'>	
			<div class="TestNameHeader" >
				<xsl:value-of select="@name" />
			</div>
			<xsl:apply-templates select="./result" />
		</div>
	</xsl:template>

	
	
	<xsl:template match="result">	
		<div align="left" style='padding-left:10px;padding-bottom:3px;'>
			<table cellpadding='0' cellspacing='0' Class='ResultText' style='table-layout:fixed'>
				<xsl:choose>
					<xsl:when test='@abnormal = "1"'>
					     <xsl:attribute name="Class">
								AbnormalResultText
					     </xsl:attribute>
					</xsl:when>
					<xsl:otherwise>
						<xsl:attribute name="Class">
								ResultText
					         </xsl:attribute>
					</xsl:otherwise>
				</xsl:choose>
				<colgroup>
					<col align='left' width='400px' />
					<col align='left' width='60px' />					
					<col align='left' width='180px' />			
					<col align='left' width='40px' />
				</colgroup>
				<tbody>
					<tr>
						<td valign="top">
							<span style="width:400px;overflow-x:hidden;word-wrap:break-word;">
								<xsl:if test='../@labgenerated = "1"'>
									labgenerated
									<xsl:text>&#160;&#160;&#160;&#160;&#160;</xsl:text>
								</xsl:if>
								<xsl:value-of select="@name" />
							</span>
						</td>
						<td valign="top">
							<span style="width:50px;overflow-x:hidden;word-wrap:break-word;">
								<xsl:apply-templates select="./flags" />
							</span>
						</td>						
						<td valign="top">
							<span style="width:170px;overflow-x:hidden;word-wrap:break-word;">
								<xsl:if test='@range != ""' >
									<xsl:value-of select="@range" />
									<xsl:text> </xsl:text>
									<xsl:value-of select="@units" />
							</xsl:if>
							</span>
						</td>
						<td valign="top">
							<span style="width:50px;overflow-x:hidden;word-wrap:break-word;">
							<xsl:value-of select="@labid" />
							</span>
						</td>
					</tr>
				</tbody>
			</table>
		</div>
		
			<div align="left">
				
				<xsl:apply-templates select="./notes" />
			</div>
			<br/>
		
	</xsl:template>
	





	<xsl:template match="flags">
		<xsl:for-each select='./flag' >
			<xsl:value-of select='@value' />
			<xsl:if test="position()!=last()">, </xsl:if>
		</xsl:for-each>
	</xsl:template>





	<xsl:template match="labcompany">
		<table border='0'>
			<tr>
				<td width='75px' >
					<span class='Label'>Lab Name: </span>
					
				</td>
				<td style="padding-right:30px;">
					<span  class='DataText' >
						<xsl:value-of select="@name"/>	
						<xsl:if test="@id != ''">
							<xsl:text> (</xsl:text>
							<xsl:value-of select="@id" /> 
							<xsl:text>)</xsl:text>
						</xsl:if>
					</span>								
				</td>
				<td width='50px' >
					<span class='Label'>Director: </span>
					
				</td>
				<td >
					<span  class='DataText' >
						<xsl:if test="normalize-space(./Contact/@FirstName) != '.'">
							<xsl:if test="./Contact/@FirstName != ''">
								<xsl:value-of select="./Contact/@FirstName" />
								<xsl:text> </xsl:text>	
							</xsl:if>
						</xsl:if>
						
						<xsl:if test="normalize-space(./Contact/@MiddleName) != '.'">
							<xsl:if test="./Contact/@MiddleName != ''">
								<xsl:value-of select="./Contact/@MiddleName" />
								<xsl:text> </xsl:text>	
							</xsl:if>
						</xsl:if>
						
						<xsl:if test="normalize-space(./Contact/@LastName) != '.'">
							<xsl:if test="./Contact/@LastName != ''">
								<xsl:value-of select="./Contact/@LastName" />
								<xsl:text> </xsl:text>	
							</xsl:if>
						</xsl:if>
						
						<xsl:if test="normalize-space(./Contact/@Suffix) != '.'">
							<xsl:if test="./Contact/@Suffix != ''">
								<xsl:value-of select="./Contact/@Suffix" />	
							</xsl:if>
						</xsl:if>
						
						<xsl:if test="normalize-space(./Contact/@Degree) != '.'">
							<xsl:if test="./Contact/@Degree != ''">
								<xsl:text>, </xsl:text>
								<xsl:value-of select="./Contact/@Degree" />
								<xsl:text> </xsl:text>	
							</xsl:if>
						</xsl:if>
						
						
					</span>								
				</td>
			</tr>
			<tr>
				<td valign='top' width='75px' >
					<span class='Label'>Address: </span>
				</td>
				<td valign='top' style="padding-right:30px;">
					<span  class='DataText' >
						<xsl:apply-templates select="./address" />	
					</span>								
				</td>
				<td valign='top' width='60px' >
					<span class='Label'>Phone #: </span>
				</td>
				<td valign='top' style="padding-right:30px;">
					<span  class='DataText' >
						<xsl:value-of select="@phone" />
					</span>								
				</td>
			</tr>
		</table>
		<br/>		
	</xsl:template>




	<xsl:template match="address">
		<table Class='Datatext' border='0' cellpadding='0' cellspacing='0'>
			<tr>
				<td width='40px'></td>
				<td width='20px'></td>
				<td width='20px'></td>
			</tr>
			<tr>
				<td colspan='3' width='160'>
					<xsl:value-of select="@addressline1"/>
				</td>
				<td></td>
			</tr>
			<tr>
				<td>
					<xsl:value-of select="@city"/>
					<xsl:if test="@city != ''">
						<xsl:text>,</xsl:text>
					</xsl:if>
					<xsl:text> </xsl:text>
				</td>
				<td>
					<xsl:value-of select="@state"/>
					<xsl:text>  </xsl:text>
				</td>
				<td>
					<xsl:value-of select="@zipcode"/>
				</td>
			</tr>
		</table>
	</xsl:template>

	
	
	<xsl:template  match="notes">
		<xsl:for-each select='./line' >
			<span  class='DataText' style="width:650px;margin-left:20px;word-wrap:break-word;">
				<pre style="display:inline; font-family:'Lucida Console'; font-Size:11px;">
					<xsl:value-of select="@value" disable-output-escaping="yes"/>
					<br/>
				</pre>
			</span>
		</xsl:for-each>
		<br/>
	</xsl:template>


</xsl:stylesheet>

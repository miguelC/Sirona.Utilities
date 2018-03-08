<?xml version='1.0'?>

<!DOCTYPE xsl:stylesheet [<!ENTITY CDA-Stylesheet '-//HL7//XSL HL7 V1.1 CDA Stylesheet: 2000-08-03//EN'>]>
<xsl:stylesheet version='1.0' xmlns:xsl='http://www.w3.org/1999/XSL/Transform'>
<xsl:output method='html' indent='yes' version='4.01' doctype-public='-//W3C//DTD HTML 4.0 Transitional//EN'/>
<xsl:variable name='docType' select='/levelone/clinical_document_header/document_type_cd/@DN'/>
<xsl:variable name='orgName' select='/levelone/clinical_document_header/originating_organization/organization/organization.nm/@V'/>
<xsl:variable name='title'>
	<xsl:value-of select='$orgName'/>
	<xsl:text> </xsl:text>
	<xsl:value-of select='$docType'/>
</xsl:variable>
<xsl:template match='/levelone'>
	<html xmlns="http://www.w3.org/1999/xhtml">
		<head>
			<meta name='format-detection' content='telephone=no'/>
      <style type="text/css" media="screen">
          body {background-color: transparent; color: black; FONT-SIZE: 10pt;FONT-FAMILY: Tahoma }
        div.paragraphHeader{ margin: 0px 0 0px 10px;font-Size:12px; font-Family:tahoma; padding-left:0px; }
        div.paragraph{ margin: 0px 0 5px 5px;font-Size:12px; font-Family:tahoma;padding-left:5px; }
        div.caption { font-weight: bold; text-decoration: underline;font-Size:14px; font-Family:tahoma; letter-spacing:2px; color:#003366 }
        span.caption {align:left;text-align:left;font-weight: bold; font-family: tahoma; font-size: 12px;color:#000000; }
        span.data { font-family: tahoma; font-size: 12px;color:#000000; }
        div.title { font-weight: bold;text-align:left;font-family: tahoma;font-size:22px;color:#003366; }
        div.demographics { text-align: center ;width:100%; }
        div.demographicsslim { text-align: left ;width:100%;padding-left:8px; }
        th.header { font-weight: bold; font-family: tahoma; font-size: 12px;color:#000000; }
        td.header { font-family: tahoma; font-size: 12px;color:#000000; }
        th{ font-weight: bold; font-family: tahoma; font-size: 12px;color:#000000; }
        td{ font-family: tahoma; font-size: 12px;color:#000000; }
        li.caption {align:left;font-weight:bold; font-family: tahoma; font-size: 12px;color:#000000; }
        li.subcaption { font-family: tahoma; font-size: 12px;color:#000000; }
        table.DataCell{text-align:left;padding-left:0px;cursor:hand;font-size:12px;font-family:tahoma;line-height:20px;}
        td.subheadVitals{color:#707c88;}
        td.printclass{font-size:8pt;}
        table {cell-padding: 0px;}
        A
        {
        COLOR: black;
        FONT-FAMILY: Tahoma;
        TEXT-DECORATION: none
        }
        A:active
        {
        COLOR: black;
        FONT-FAMILY: Tahoma;
        TEXT-DECORATION: none
        }
        A:link
        {
        COLOR: black;
        FONT-FAMILY: Tahoma;
        TEXT-DECORATION: none
        }
        A:visited
        {
        COLOR: black;
        FONT-FAMILY: Tahoma;
        TEXT-DECORATION: none
        }
        ADDRESS
        {
        COLOR: black;
        FONT-FAMILY: Tahoma;
        TEXT-DECORATION: none
        }
        font.ABNORMALFONT
        {
        color:#b22222;
        font-style:italic;
        }

        .InputSpan
        {
        position:absolute;
        left:0px;
        top:0px;
        width:698px;
        font-Size:10pt;
        font-Family:tahoma;
        padding:3px;
        z-Index:100;
        color:black;
        background-color:transparent;
        border:0;
        overflow-y: visible;
        }
        .InputSpanCustomNote
        {
        position:absolute;
        left:0px;
        top:0px;
        width:698px;
        font-Size:10pt;
        font-Family:tahoma;
        padding:3px;
        z-Index:100;
        color:black;
        background-color:transparent;
        border:0;
        overflow-y: visible;
        }

      </style>
			<title>
				<xsl:if test='clinical_document_header/document_type_cd/@gmtid != "16"'>
					<xsl:text>[</xsl:text>
					<xsl:value-of select='$title'/><xsl:text>] [</xsl:text>
					<xsl:for-each select='/levelone/clinical_document_header/patient/person'>
						<xsl:call-template name='getName'/>
					</xsl:for-each>
					<xsl:text>] [</xsl:text>
					<xsl:value-of select='/levelone/clinical_document_header/patient/person/id/@EX'/>	
					<xsl:text>] </xsl:text>
				</xsl:if>
			
			</title>
		</head>
		<body  style="font-Family:tahoma;width:100%;">
			<div id='divBodyContainer' dir='ltr'>		
				<xsl:if test='clinical_document_header/document_type_cd/@gmtid = "16" or clinical_document_header/document_type_cd/@gmtid = "11"'>
				<br/>
				<br/>
				</xsl:if>
				
				<xsl:if test='clinical_document_header/document_type_cd/@gmtid = "11"'>
					<div class='title'>
						<xsl:value-of select='$title'/>
					</div>
					<br/>
					<br/>
				</xsl:if>
				
				<xsl:if test='clinical_document_header/document_type_cd/@gmtid = "18"'>
					<xsl:apply-templates select='clinical_document_header' mode='slim'/>
				</xsl:if>
				
				<xsl:if test='clinical_document_header/document_type_cd/@gmtid != "18"  and clinical_document_header/document_type_cd/@gmtid != "16" and clinical_document_header/document_type_cd/@gmtid != "11"'>
					<div class='title'>
						<xsl:value-of select='$title'/>
					</div>
					<br/>
					<xsl:apply-templates select='clinical_document_header' mode='normal'/>
					<br/>
				</xsl:if>
				
				<xsl:apply-templates select='body'/>
				
				<xsl:if test='clinical_document_header/document_type_cd/@gmtid != "16" and  clinical_document_header/document_type_cd/@gmtid != "11"'>
					<xsl:if test='clinical_document_header/document_type_cd/@gmtid != "18"' >
						<br/><br/><br/>
						<xsl:for-each select='/levelone/clinical_document_header/legal_authenticator/person'>
							
							<xsl:if test="position()=1">
								<xsl:call-template name='signature'/>
							</xsl:if>
							
							<xsl:if test="position()!=1">
								<xsl:call-template name='cosignature'/>
							</xsl:if>
							
						</xsl:for-each>
						<br/>
						<br/>
					</xsl:if>
					<xsl:if test='clinical_document_header/document_type_cd/@gmtid = "18"' >

						<xsl:for-each select='/levelone/clinical_document_header/legal_authenticator/person'>
							
							<xsl:if test="position()=1">
								<xsl:call-template name='signature'/>
							</xsl:if>
							
							<xsl:if test="position()!=1">
								<xsl:call-template name='cosignature'/>
							</xsl:if>
							
						</xsl:for-each>
						<br/>
					</xsl:if>
				</xsl:if>
			</div>
		</body>
	</html>

</xsl:template>

<!--
     generate a table at the top of the document containing
	 encounter and patient information.  Encounter info is
	 rendered in the left column(s) and patient info is
	 rendered in the right column(s).
	 
	 This assumes several things about the source document which
	 won't be true in the general case:
	 
	 	1. there is only 1 of everything (i.e., physcian, patient, etc.)
		2. I haven't bothered to map all HL7 table values
		   (e.g., actor.type_cd and administrative_gender_cd)
		   and have only those that are used in the sample document
		
		I tried to do the table formting with CSS2 rules, but Netscape
		doesn't seem to handle the table rules well (or at all:-( so I
		just gave up
  -->
<xsl:template match='clinical_document_header' mode='normal'>
	<div style='display:block;' class='demographics' >
		<table border='0' >
			<tr>
				<td align='left' vAlign='top'>
					<table border='0' width='330px'>
						<tbody>
							<xsl:call-template name='patient'/>
						</tbody>
					</table>
				</td>
				<td align='left' vAlign='top'>
					<table border='0' width='330px' >
						<tbody>
							<xsl:call-template name='encounter'/>
						</tbody>
					</table>
				</td>
			</tr>
		</table>
	</div>
</xsl:template>

<xsl:template match='clinical_document_header' mode='slim'>
	<div class='demographicsslim' >
		<table border='0' width='700' style='table-layout:fixed;'>
			<tbody>
				<tr>
					<xsl:call-template name='patient_slim'/>
				</tr>
				<tr>
					<xsl:call-template name='encounter_slim'/>
				</tr>
			</tbody>
		</table>			
	</div>
</xsl:template>
	
<xsl:template name='encounter_slim'>
		<xsl:apply-templates select='patient_encounter/encounter_tmr'/>
	
		<xsl:apply-templates select='provider'/>
	
		<xsl:apply-templates select='patient_encounter/service_location'/>
</xsl:template>

	
<xsl:template name='encounter'>
	<tr>
		<xsl:apply-templates select='patient_encounter/encounter_tmr'/>
	</tr>
	<tr>
		<xsl:apply-templates select='provider'/>
	</tr>
	<tr>
		<xsl:apply-templates select='patient_encounter/service_location'/>
	</tr>
</xsl:template>

<xsl:template match='encounter_tmr'>
<xsl:if test='@visit = "true"'>		
	<th align='left' width='88px'  class='header'>Visit Date:</th>
</xsl:if>
<xsl:if test='@visit = "false"'>		
	<th align='left' width='88px'  class='header'>Create Date:</th>
</xsl:if>

	<td align='left' width='250px'  class='header'>
		<xsl:call-template name='date'>
			<xsl:with-param name='date' select='@V'/>
		</xsl:call-template>
	</td>
</xsl:template>

<xsl:template match='service_location'>
	<th align='left' width='88px'  class='header'>Location:</th>
	<td align='left' width='250px'  class='header'>
		<xsl:value-of select='@V'/>
	</td>
</xsl:template>

<xsl:template match='provider'>
	<th align='left' width='88px'>
		<xsl:call-template name='provider_type_cd'>
			<xsl:with-param name='type_cd' select='provider.type_cd/@V'/>
		</xsl:call-template>
		<xsl:text>Provider:</xsl:text>
	</th>
	<td align='left' width='250px'>
		<xsl:variable name='ptr' select='person/id/@EX'/>

		<xsl:for-each select='/levelone/clinical_document_header/provider/person[id/@EX=$ptr]'>
			<xsl:call-template name='getName'/>
		</xsl:for-each>
	</td>
</xsl:template>

<xsl:template name='patient' >
	<xsl:apply-templates select='patient' mode='normal'/>
	<tr>
		<xsl:apply-templates select='patient/birth_dttm'/>
	</tr>
</xsl:template>

<xsl:template name='patient_slim'>
	<xsl:apply-templates select='patient' mode='slim'/>
</xsl:template>


<xsl:template match='birth_dttm'>
	<th align='left' width='120px' class='header'>Birthdate:</th>
	<td align='left' width='230px' class='header'>
		<xsl:call-template name='date'>
			<xsl:with-param name='date' select='@V'/>
		</xsl:call-template>
	</td>
</xsl:template>

<xsl:template match='patient' mode='normal'>
	<tr>
		<th align='left' width='120px' class='header'>Patient Name:</th>
		<td align='left' width='260px' class='header'>
			<xsl:for-each select='person'>
				<xsl:call-template name='getName'/>
			</xsl:for-each>
		</td>
	</tr>
	<tr>
		<th align='left' width='120px' class='header'>
			<xsl:text>Patient ID:</xsl:text>
		</th>
		<td align='left' width='230px' class='header'>
			<xsl:value-of select='person/id/@EX'/>				
		</td>
	</tr>
	<tr>
		<th align='left' width='120px' class='header'>Sex:</th>
		<td align='left' width='230px' class='header'>
			<xsl:call-template name='administrative_gender_cd'>
				<xsl:with-param name='gender_cd' select='administrative_gender_cd/@V'/>
			</xsl:call-template>
		</td>
	</tr>
</xsl:template>


<xsl:template match='patient' mode='slim'>
	
		<th align='left' width='96px' class='header'>Patient Name:</th>
		<td align='left' width='153px' class='header'>
			<xsl:for-each select='person'>
				<xsl:call-template name='getName'/>
			</xsl:for-each>
		</td>
	
		<th align='left' width='80px' class='header'>
			<xsl:text>Patient ID:</xsl:text>
		</th>
		<td align='left' width='153px' class='header'>
			<xsl:value-of select='person/id/@EX'/>				
		</td>
	
		<th align='left' width='60px' class='header'>Sex:</th>
		<td align='left' width='155px' class='header'>
			<xsl:call-template name='administrative_gender_cd'>
				<xsl:with-param name='gender_cd' select='administrative_gender_cd/@V'/>
			</xsl:call-template>
		</td>
</xsl:template>


<!--
    just apply the default template for these
  -->
<xsl:template match='body|caption|content'>
	<xsl:apply-templates/>
</xsl:template>

<!--
    spit out the caption (in the 'caption' style)
	followed by applying whatever templates we
	have for the applicable children
  -->
<xsl:template match='section'>
	<div>
		<xsl:if test='caption != "Custom Note"'>
			<div class='caption'>
				<xsl:apply-templates select='caption'/>
			</div>
		</xsl:if>
		
		
		<xsl:apply-templates select='paragraph|list|table|section|local_markup'/>
	</div><br/>
</xsl:template>

<xsl:template match='section/section'>
	<ul style='margin-top:0;margin-bottom:0;padding-top:0;padding-bottom:0;'>
		<li style='list-style-type:none;' >
			<span class='caption' style='align:left;'>
				<xsl:apply-templates select='caption'/>
				<xsl:if test="@caption!=''">:</xsl:if>
			</span>
			<xsl:apply-templates select='paragraph|list|table|section'/>
		</li>
	</ul>
</xsl:template>

<xsl:template match='focused'>
	<b>
		<i>
			<xsl:apply-templates/>
		</i>
	</b>
</xsl:template>


<!--
    currently ignores paragraph captions...
	
	I need samples of the use description and render
	to know what really should be done with them
  -->
<xsl:template match='paragraph'>
	<xsl:if test="@title != ''">
			<div  class='paragraphHeader'  style='font-weight:bold;width:85%'>
			<xsl:value-of select='@title'/>
			</div>
	</xsl:if>
	<xsl:if test="@title = ''">
			<div  class='paragraphHeader'  style='font-weight:bold;width:85%'>
			</div>
	</xsl:if>
	<div class='paragraph' style='width:85%'>
		<xsl:apply-templates select='content'/>
	</div>
	<xsl:apply-templates select='local_markup'/>
</xsl:template>

<xsl:template match='bolditalic'>
	<b><i>
		<xsl:apply-templates select='content'/>
	</i></b>
</xsl:template>

<xsl:template match='font'>
	<font class='ABNORMALFONT'>
		<xsl:apply-templates/>
	</font>
</xsl:template>
<!--
    currently ignore caption's on the list itself,
	but handles them on the list items
  -->
<xsl:template match='list'>
	<ul>
		<xsl:for-each select='item'>
			<li >
				<xsl:if test='caption'>
					<span  class='caption'>
						<xsl:apply-templates select='caption'/>
					</span>
					<xsl:text> : </xsl:text>
				</xsl:if>
				<xsl:apply-templates select='content'/>
			</li>
		</xsl:for-each>
	</ul>
</xsl:template>





<xsl:template match='section/section/paragraph'>
	<span class='data' >
		<xsl:apply-templates/>
	</span>
</xsl:template>

<!-- 
     Tables
	 
     just copy over the entire subtree, as is
	 except that the children of CAPTION are possibly handled by
	 other templates in this stylesheet
  -->
  
 <xsl:template match='table'>
	<xsl:copy>
		<xsl:apply-templates select='*|@*|text()'/>
	</xsl:copy>
	<BR/>
</xsl:template> 

<xsl:template match='th'>
	<xsl:copy>
		
		<xsl:attribute name="class">RowHeader</xsl:attribute>
		<xsl:apply-templates select='*|@*|text()'/>
	</xsl:copy>
</xsl:template>



<xsl:template match='caption|thead|tfoot|tbody|colgroup|col|tr|td|br'>
	<xsl:copy>
		<xsl:apply-templates select='*|@*|text()'/>
	</xsl:copy>
	
</xsl:template>

<xsl:template match='table/@*|thead/@*|tfoot/@*|tbody/@*|colgroup/@*|col/@*|tr/@*|th/@*|td/@*'>
	<xsl:copy>
		<xsl:apply-templates/>
	</xsl:copy>
</xsl:template>

<!--
     this currently only handles GIF's and JPEG's.  It could, however,
	 be extended by including other image MIME types in the predicate
	 and/or by generating <object> or <applet> tag with the correct
	 params depending on the media type
  -->
<xsl:template match='observation_media'>
	<xsl:if test='observation_media.value[
			@MT="image/gif" or @MT="image/jpeg"
			]'>
		<br clear='all'/>
		<xsl:element name='img'>
			<xsl:attribute name='src'>
				<xsl:value-of select='observation_media.value/REF/@V'/>
			</xsl:attribute>
		</xsl:element>
	</xsl:if>
</xsl:template>

<!--
	turn the link_html subelement into an HTML a element,
	complete with any attributes and content it may have,
	while stripping off any CDA specific attributes
  -->
<xsl:template match='link'>
	<xsl:element name='a'>
		<xsl:for-each select='link_html/@*'>
   			<xsl:if test='not(name()="originator" or name()="confidentiality")'>
				<xsl:attribute name='{name()}'>
					<xsl:value-of select='.'/>
				</xsl:attribute>
			</xsl:if>
		</xsl:for-each>
		<xsl:value-of select='link_html'/>
	</xsl:element>
</xsl:template>

<!--
    this doesn't do anything with the description
	or render attributes...it simply decides whether
	to remove the entire subtree or just pass the
	content thru
	
	I need samples of the use description and render
	to know what really should be done with them
  -->

<xsl:template match='local_markup'>
	
	<xsl:apply-templates select='datahtml/drawings' />
	
	<span language='vbscript' onclick='window.event.returnvalue=false'>
		<xsl:value-of select='datahtml' disable-output-escaping="yes"/>
	</span>
	
	<xsl:apply-templates select='datahtml/sketchpads' />
</xsl:template>

<xsl:template match="datahtml/drawings">
	<div id="drawingsContainer">
		<xsl:for-each select='drawing'>
			<xsl:for-each select='sketchpads/sketchpad'>
				<span style='font-weight:bold;'>
					Figure <xsl:for-each select="../..">
							  <xsl:number/>
							</xsl:for-each>
					<xsl:choose>
						<xsl:when test='./@seq != ""'><xsl:text>.</xsl:text><xsl:value-of select="./@seq"/></xsl:when>
						<xsl:otherwise></xsl:otherwise>
					</xsl:choose>:
				</span>
				<span style='padding-left:5px;'>
					<xsl:value-of select="../@title"/>
				</span>
				<div>
					<table>
					<tr>
						<td style="position:relative; top:0px; left:0xp;">
							<EMBED style="border:1px solid #000000" wmode='transparent' width="700" height="400" src="/PrimePractice/Clinical/SketchPad/SketchPadPreview.svg">
								<xsl:attribute name="id">
								<xsl:text>drawing</xsl:text>
								<xsl:value-of select="../../@refid"/>_<xsl:choose>
																		<xsl:when test='./@seq != ""'><xsl:value-of select="./@seq"/></xsl:when>
																			<xsl:otherwise>0</xsl:otherwise>
																		</xsl:choose>
								</xsl:attribute>
								<xsl:attribute name="name">
									<xsl:text>drawing</xsl:text>
									<xsl:value-of select="../../@refid"/>_<xsl:choose>
																		<xsl:when test='./@seq != ""'><xsl:value-of select="./@seq"/></xsl:when>
																			<xsl:otherwise>0</xsl:otherwise>
																		</xsl:choose>
								</xsl:attribute>
							</EMBED>
							<textarea class="InputSpan" onselectstart="cancelbubble" onbeforeactivate="cancelbubble" onbeforeeditfocus="cancelbubble">
								<xsl:attribute name="id">
									<xsl:text>drawingtext</xsl:text>
									<xsl:value-of select="../../@refid"/>_<xsl:choose>
																		<xsl:when test='./@seq != ""'><xsl:value-of select="./@seq"/></xsl:when>
																			<xsl:otherwise>0</xsl:otherwise>
																		</xsl:choose>
								</xsl:attribute>
								<xsl:attribute name="data">
									<xsl:value-of select="text/@data"/>
								</xsl:attribute>
								<xsl:value-of select="./text/@data"/>
							</textarea>
						</td>
					</tr>
					</table>
				</div>
			</xsl:for-each>
		</xsl:for-each>
	</div>
</xsl:template>


<xsl:template match="datahtml/sketchpads">
	<div id="sketchPadsContainer" style="padding-left:5px;">
		<xsl:for-each select='sketchpad'>
			
				<table>
				<tr>
					<td>
						<xsl:if test='../../../../caption = "Custom Note"'>
							<table>
							<tr>
								<td><span style="font-weight:bold; padding-right:10px;">Title:</span></td>
								<td><xsl:value-of select="../@title"/></td>
								<td width="100"></td>

								<td><span style="font-weight:bold; padding-right:10px;">DocType:</span></td>
								<td><xsl:value-of select="../@doctypename"/></td>
							</tr>
							</table>
						</xsl:if>
					</td>
				</tr>
				<tr>
					<td style="position:relative; top:0px; left:0px;">
						<EMBED style="border:solid #003366 1px;" wmode='transparent' width="700" height="400" src="/PrimePractice/Clinical/SketchPad/SketchPadPreview.svg">
							<xsl:attribute name="id">
								<xsl:choose>
									<xsl:when test='../../../../caption = "Custom Note"'>customnote</xsl:when>
									<xsl:otherwise><xsl:value-of select="@section"/></xsl:otherwise>
								</xsl:choose>
							
								<xsl:text>sketch</xsl:text>
								<xsl:if test='@seq != ""'><xsl:value-of select="@seq"/></xsl:if>
								<xsl:if test='@index != ""'><xsl:value-of select="@index"/></xsl:if>
							</xsl:attribute>

							<xsl:attribute name="name">
								<xsl:choose>
									<xsl:when test='../../../../caption = "Custom Note"'>customnote</xsl:when>
									<xsl:otherwise><xsl:value-of select="@section"/></xsl:otherwise>
								</xsl:choose>
								
								<xsl:text>sketch</xsl:text>
								<xsl:if test='@seq != ""'><xsl:value-of select="@seq"/></xsl:if>
								<xsl:if test='@index != ""'><xsl:value-of select="@index"/></xsl:if>
							</xsl:attribute>
						</EMBED>
						<textarea class="InputSpan" onselectstart="cancelbubble" onbeforeactivate="cancelbubble" onbeforeeditfocus="cancelbubble">
							<xsl:attribute name="id">
								<xsl:choose>
									<xsl:when test='../../../../caption = "Custom Note"'>customnote</xsl:when>
									<xsl:otherwise><xsl:value-of select="@section"/></xsl:otherwise>
								</xsl:choose>

								<xsl:text>text</xsl:text>
								<xsl:if test='@seq != ""'><xsl:value-of select="@seq"/></xsl:if>
								<xsl:if test='@index != ""'><xsl:value-of select="@index"/></xsl:if>
							</xsl:attribute>
							<xsl:attribute name="data">
								<xsl:value-of select="text/@data"/>
							</xsl:attribute>
							<xsl:value-of select="text/@data"/>
						</textarea>
					</td>
				</tr>
				</table>

		</xsl:for-each>
	</div>
</xsl:template>


<!--
<xsl:template match='local_markup'>
	<xsl:value-of select='datahtml' disable-output-escaping="yes"/>
	
</xsl:template>
-->

<!--
     elements to ignore
  -->
<xsl:template match='coded_entry'>
</xsl:template>

<xsl:template match='caption_cd'>
</xsl:template>


<!--
     template(s) to output signature block
  -->
<xsl:template name='signature'>
	<xsl:variable name='signers' select='.'/>
	<xsl:if test='$signers'>
		<div>
			<span class='caption'>Electronically Signed by: </span>
			<xsl:for-each select='$signers'>
				<xsl:call-template name='getName'>
					<xsl:with-param name='person' select='.'/>
				</xsl:call-template>
				<xsl:text> on </xsl:text>	
				
				<xsl:call-template name='date'>
					<xsl:with-param name='date' select='../participation_tmr/@V'/>
				</xsl:call-template>

              
			</xsl:for-each>
		</div>
	</xsl:if>
</xsl:template>


<xsl:template name='cosignature'>
	<xsl:variable name='signers' select='.'/>
	<xsl:if test='$signers'>
		<div>
			<span class='caption'>Electronically Co-signed by: </span>
			<xsl:for-each select='$signers'>
				<xsl:call-template name='getName'>
					<xsl:with-param name='person' select='.'/>
				</xsl:call-template>
				<xsl:text> on </xsl:text>	
				
				<xsl:call-template name='date'>
					<xsl:with-param name='date' select='../participation_tmr/@V'/>
				</xsl:call-template>
              
			</xsl:for-each>
		</div>
	</xsl:if>
</xsl:template>

<!--
     general purpose (named) templates used in multiple places
  -->
<!--
     assumes current node is a <person_name> node

     Does not handle nearly all of the complexity of the person datatype,
	 but is illustritive of what would be required to do so in the future
  -->
<xsl:template name='getName'>
	<xsl:apply-templates select='person_name[person_name.type_cd/@V="L"]'/>
</xsl:template>
<xsl:template match='person_name'>
	<xsl:choose>
		<xsl:when test='nm/GIV[@QUAL="RE"]/@V'>
			<xsl:value-of select='nm/GIV[@QUAL="RE"]/@V'/>
		</xsl:when>
		<xsl:when test='nm/GIV[@CLAS="N"]/@V'>
			<xsl:value-of select='nm/GIV[@CLAS="N"]/@V'/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select='nm/GIV/@V'/>
		</xsl:otherwise>
	</xsl:choose>

	<xsl:choose>
		<xsl:when test='nm/MID[@QUAL="RE"]/@V'>
			<xsl:text> </xsl:text>
			<xsl:value-of select='substring (nm/MID[@QUAL="RE"]/@V, 1, 1)'/>
			<xsl:text>.</xsl:text>
		</xsl:when>
		<xsl:when test='nm/MID/@V != ""'>
			<xsl:text> </xsl:text>
			<xsl:value-of select='substring (nm/MID/@V, 1, 1)'/>
			<xsl:text>.</xsl:text>
		</xsl:when>
	</xsl:choose>

	<xsl:choose>
		<xsl:when test='nm/FAM[@QUAL="RE"]/@V'>
			<xsl:text> </xsl:text>
			<xsl:value-of select='nm/FAM[@QUAL="RE"]/@V'/>
		</xsl:when>
		<xsl:otherwise>
			<xsl:text> </xsl:text>
			<xsl:value-of select='nm/FAM/@V'/>
		</xsl:otherwise>
	</xsl:choose>

	<xsl:choose>
		<xsl:when test='nm/SFX[@QUAL="RE"]/@V'>
			<xsl:text>, </xsl:text>
			<xsl:value-of select='nm/SFX[@QUAL="RE"]/@V'/>
		</xsl:when>
		<xsl:when test='nm/SFX/@V != ""'>
			<xsl:text>, </xsl:text>
			<xsl:value-of select='nm/SFX/@V'/>
		</xsl:when>
	</xsl:choose>
	
</xsl:template>

<!--
     outputs a date in Month Day, Year form
	 
	 e.g., 19991207  ==> December 07, 1999
  -->
<xsl:template name='date'>
	<xsl:param name='date'/>
	<xsl:variable name='month' select='substring ($date, 6, 2)'/>
	<xsl:choose>
		<xsl:when test='$month=01'>
			<xsl:text>January </xsl:text>
		</xsl:when>
		<xsl:when test='$month=02'>
			<xsl:text>February </xsl:text>
		</xsl:when>
		<xsl:when test='$month=03'>
			<xsl:text>March </xsl:text>
		</xsl:when>
		<xsl:when test='$month=04'>
			<xsl:text>April </xsl:text>
		</xsl:when>
		<xsl:when test='$month=05'>
			<xsl:text>May </xsl:text>
		</xsl:when>
		<xsl:when test='$month=06'>
			<xsl:text>June </xsl:text>
		</xsl:when>
		<xsl:when test='$month=07'>
			<xsl:text>July </xsl:text>
		</xsl:when>
		<xsl:when test='$month=08'>
			<xsl:text>August </xsl:text>
		</xsl:when>
		<xsl:when test='$month=09'>
			<xsl:text>September </xsl:text>
		</xsl:when>
		<xsl:when test='$month=10'>
			<xsl:text>October </xsl:text>
		</xsl:when>
		<xsl:when test='$month=11'>
			<xsl:text>November </xsl:text>
		</xsl:when>
		<xsl:when test='$month=12'>
			<xsl:text>December </xsl:text>
		</xsl:when>
	</xsl:choose>
	<xsl:choose>
		<xsl:when test='substring ($date, 9, 1)="0"'>
			<xsl:value-of select='substring ($date, 10, 1)'/><xsl:text>, </xsl:text>
		</xsl:when>
		 
		<xsl:otherwise>
			<xsl:if test='substring ($date, 9, 1) != ""'>
				<xsl:value-of select='substring ($date, 9, 2)'/><xsl:text>, </xsl:text>
			</xsl:if>
		</xsl:otherwise>
		
	</xsl:choose>
	<xsl:value-of select='substring ($date, 1, 4)'/><xsl:text> </xsl:text>
	<xsl:value-of select='substring ($date, 11)'/>
</xsl:template>

<!--
     table lookups
  -->
<xsl:template name='provider_type_cd'>
	<xsl:param name='type_cd'/>
	<xsl:choose>
		<xsl:when test='$type_cd="CON"'>
			<xsl:text>Consultant</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="PRISURG"'>
			<xsl:text>Primary surgeon</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="FASST"'>
			<xsl:text>First assistant</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="SASST"'>
			<xsl:text>Second assistant</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="SNRS"'>
			<xsl:text>Scrub nurse</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="TASST"'>
			<xsl:text>Third assistant</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="NASST"'>
			<xsl:text>Nurse assistant</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="ANEST"'>
			<xsl:text>Anesthetist</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="ANRS"'>
			<xsl:text>Anesthesia nurse</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="MDWF"'>
			<xsl:text>Midwife</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="ATTPHYS"'>
			<xsl:text>Attending physician</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="ADMPHYS"'>
			<xsl:text>Admitting physician</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="DISPHYS"'>
			<xsl:text>Discharging physician</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="RNDPHYS"'>
			<xsl:text>Rounding physician</xsl:text>
		</xsl:when>
		<xsl:when test='$type_cd="PCP"'>
			<xsl:text>Primary care provider</xsl:text>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select='$type_cd'/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

<xsl:template name='administrative_gender_cd'>
	<xsl:param name='gender_cd'/>
	<xsl:choose>
		<xsl:when test='$gender_cd="M"'>
			<xsl:text>Male</xsl:text>
		</xsl:when>
		<xsl:when test='$gender_cd="F"'>
			<xsl:text>Female</xsl:text>
		</xsl:when>
		<xsl:otherwise>
			<xsl:value-of select='$gender_cd'/>
		</xsl:otherwise>
	</xsl:choose>
</xsl:template>

</xsl:stylesheet>

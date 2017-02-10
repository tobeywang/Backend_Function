protected void EditBtn_Click(object sender, EventArgs e)
        {
            int sPicHeightSize = 150;

            //File Update 
            var files = Request.Files;
            if (files.Count > 0)
            {
                var fileList = new List<FileItem>();
                FileItem file;
                string docId = string.Format(MachineRent.Properties.Settings.Default.FileDocIdDef, ViewState["Id"]);
                for (int icount = 0; icount < files.Count; icount++)
                {
                    file = new FileItem();

                    var fileReal = new System.IO.FileInfo(files[icount].FileName);
                    using (var picImage = System.Drawing.Image.FromStream(fileReal.OpenRead()))
                    {
                        using (var sImageOut = new System.Drawing.Bitmap(picImage, (int)((double)sPicHeightSize / (double)picImage.Size.Height * (double)picImage.Size.Width), sPicHeightSize))
                        {
                            //重複名稱處理
                            var count = 1;
                            var tempFileName = fileReal.Name;//.Replace(file.Extension, "");// System.IO.Path.GetFileName(files[icount].FileName);
                            var originalFileName = tempFileName;
                            //var fileType = fileReal.Extension; //tempFileName.Substring(tempFileName.LastIndexOf('.') + 1);
                            if (!string.IsNullOrEmpty(tempFileName))
                            {
                                while (System.IO.File.Exists(MachineRent.Properties.Settings.Default.UploadServer + tempFileName))
                                {
                                    tempFileName = string.Format("{0}({1}).{2}", originalFileName.Replace(fileReal.Extension, ""), count, fileReal.Extension);// originalFileName.Replace(string.Format(".{0}", fileType),
                                    //string.Format("({0}).{1}", count, file.Extension));
                                    count++;
                                }

                                //files[icount].SaveAs(MachineRent.Properties.Settings.Default.UploadServer + tempFileName);
                                sImageOut.Save(System.IO.Path.Combine(MachineRent.Properties.Settings.Default.UploadServer, tempFileName + "_s" + fileReal.Extension), System.Drawing.Imaging.ImageFormat.Jpeg);
                                //Format MachineRent_圖檔屬性(EX:樣品機-縮圖、正常圖)_對應DB資料Id
                                file.DocumentId = docId;
                                file.FileName = tempFileName;
                                //Format 被租物件總名_圖檔屬性_上傳固定字
                                file.Note = string.Format("{0}上傳", "樣品機");
                                file.OriginalFileName = originalFileName;
                                //Format /WebApp/PD/Tool/MachineRent/Pic  路徑
                                file.Portal = MachineRent.Properties.Settings.Default.PortalUpload;
                                file.FilePath = MachineRent.Properties.Settings.Default.UploadServer;
                                file.Type = "PD";
                                file.SubType = files.AllKeys[icount].IndexOf("Multi") >= 0 ? "Multi" : "Mini";
                                file.UpdateBy = Context.User.Identity.Name;
                                file.UpdateTime = DateTime.Now;
                                fileList.Add(file);
                            }
                        }
                    }
                }
                if (fileList.Count > 0)
                {
                    var newFileService = new FileService.FileServerEnhanceOperatorSoapClient();
                    var msg = newFileService.WriteNewFileItem(fileList.ToArray());
                    //if (msg.ToLower().Contains("successful"))
                }

            }
            CloseWindow();
        }

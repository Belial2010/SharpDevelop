﻿// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Parser;

namespace ICSharpCode.SharpDevelop.Workbench
{
	/// <summary>
	/// Options for use with <see cref="OpenedFile.GetModel"/>
	/// </summary>
	[Flags]
	public enum GetModelOptions
	{
		None = 0,
		/// <summary>
		/// Return stale models without reloading.
		/// </summary>
		AllowStale = 1,
		/// <summary>
		/// Do not load any models:
		/// Returns null if the model is not already loaded, or if it is stale and the AllowStale option isn't in use.
		/// </summary>
		DoNotLoad = 2,
	}
	
	/// <summary>
	/// Option that control how <see cref="OpenedFile.ReplaceModel"/> handles the dirty flag.
	/// </summary>
	public enum ReplaceModelMode
	{
		/// <summary>
		/// The new model is marked as dirty; and any other models are marked as stale.
		/// </summary>
		SetAsDirty,
		/// <summary>
		/// The new model is marked as valid; the status of any other models is unchanged.
		/// </summary>
		SetAsValid,
		/// <summary>
		/// The new model is marked as dirty, the previously dirty model is marked as stale, and any other models are unchanged.
		/// This mode is intended for use in <see cref="IFileModelProvider{T}.Save"/> implementations.
		/// </summary>
		TransferDirty
	}
	
	/// <summary>
	/// Represents an opened file.
	/// </summary>
	public abstract class OpenedFile : ICanBeDirty
	{
		#region IsDirty implementation
		bool isDirty;
		public event EventHandler IsDirtyChanged;
		
		/// <summary>
		/// Gets/sets if the file is has unsaved changes.
		/// </summary>
		public bool IsDirty {
			get { return isDirty;}
			private set {
				if (isDirty != value) {
					isDirty = value;
					
					if (IsDirtyChanged != null) {
						IsDirtyChanged(this, EventArgs.Empty);
					}
				}
			}
		}
		#endregion
		
		#region FileName
		/// <summary>
		/// Gets if the file is untitled. Untitled files show a "Save as" dialog when they are saved.
		/// </summary>
		public bool IsUntitled {
			get {
				return fileName.ToString().StartsWith("untitled:", StringComparison.Ordinal);
			}
		}
		
		FileName fileName;
		
		/// <summary>
		/// Gets the name of the file.
		/// </summary>
		public FileName FileName {
			get { return fileName; }
			set {
				if (fileName != value) {
					ChangeFileName(value);
				}
			}
		}
		
		/// <summary>
		/// Occurs when the file name has changed.
		/// </summary>
		public event EventHandler FileNameChanged;
		
		protected virtual void ChangeFileName(FileName newValue)
		{
			SD.MainThread.VerifyAccess();
			
			fileName = newValue;
			
			if (FileNameChanged != null) {
				FileNameChanged(this, EventArgs.Empty);
			}
		}
		#endregion
		
		/// <summary>
		/// This method sets all models to 'stale', causing the file to be re-loaded from disk
		/// on the next GetModel() call.
		/// </summary>
		/// <exception cref="InvalidOperationException">The file is untitled.</exception>
		public void ReloadFromDisk()
		{
			if (IsUntitled)
				throw new InvalidOperationException("Cannot reload an untitled file from disk.");
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Saves the file to disk.
		/// </summary>
		/// <remarks>If the file is saved successfully, the dirty flag will be cleared (the dirty model becomes valid instead).</remarks>
		/// <exception cref="InvalidOperationException">The file is untitled.</exception>
		public void SaveToDisk()
		{
			if (IsUntitled)
				throw new InvalidOperationException("Cannot reload an untitled file from disk.");
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Changes the file name, and saves the file to disk.
		/// </summary>
		/// <remarks>If the file is saved successfully, the dirty flag will be cleared (the dirty model becomes valid instead).</remarks>
		public void SaveToDisk(FileName fileName)
		{
			this.FileName = fileName;
			SaveToDisk();
		}
		
		/// <summary>
		/// Saves a copy of the file to disk. Does not change the name of the OpenedFile to the specified file name, and does not reset the dirty flag.
		/// </summary>
		public void SaveCopyAs(FileName fileName)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Retrieves a file model, loading it if necessary.
		/// </summary>
		/// <param name="modelProvider">The model provider for the desired model type. Built-in model providers can be found in the <see cref="FileModels"/> class.</param>
		/// <param name="options">Options that control how</param>
		/// <returns>The model instance, or possibly <c>null</c> if <c>GetModelOptions.DoNotLoad</c> is in use.</returns>
		/// <exception cref="IOException">Error loading the file.</exception>
		/// <exception cref="FormatException">Cannot construct the model because the underyling data is in an invalid format.</exception>
		public T GetModel<T>(IFileModelProvider<T> modelProvider, GetModelOptions options = GetModelOptions.None) where T : class
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Sets the model associated with the specified model provider to be dirty.
		/// All other models are marked as stale. If another model was previously dirty, those earlier changes will be lost.
		/// </summary>
		public void MakeDirty<T>(IFileModelProvider<T> modelProvider) where T : class
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Unloads the model associated with the specified model provider.
		/// Unloading the dirty model will cause changes to be lost.
		/// </summary>
		public void UnloadModel<T>(IFileModelProvider<T> modelProvider) where T : class
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Replaces the model associated with the specified model provider with a different instance.
		/// </summary>
		/// <param name="modelProvider">The model provider for the model type.</param>
		/// <param name="model">The new model instance.</param>
		/// <param name="mode">Specifies how the dirty flag is handled during the replacement.
		/// By default, the new model is marked as dirty and all other models are marked as stale.
		/// In <see cref="IFileModelProvider{T}.Save"/> implementations, you should use <see cref="ReplaceModelMode.TransferDirty"/> instead.</param>
		public void ReplaceModel<T>(IFileModelProvider<T> modelProvider, T model, ReplaceModelMode mode = ReplaceModelMode.SetAsDirty) where T : class
		{
			throw new NotImplementedException();
		}
		
	}
	
	/*
	/// <summary>
	/// Represents an opened file.
	/// </summary>
	public abstract class OpenedFile : ICanBeDirty
	{
		protected IViewContent currentView;
		bool inLoadOperation;
		bool inSaveOperation;
		
		/// <summary>
		/// holds unsaved file content in memory when view containing the file was closed but no other view
		/// activated
		/// </summary>
		byte[] fileData;
		
		
		/// <summary>
		/// Use this method to save the file to disk using a new name.
		/// </summary>
		public void SaveToDisk(FileName newFileName)
		{
			this.FileName = newFileName;
			this.IsUntitled = false;
			SaveToDisk();
		}
		
		public abstract void RegisterView(IViewContent view);
		public abstract void UnregisterView(IViewContent view);
		
		public virtual void CloseIfAllViewsClosed()
		{
		}
		
		/// <summary>
		/// Forces initialization of the specified view.
		/// </summary>
		public virtual void ForceInitializeView(IViewContent view)
		{
			if (view == null)
				throw new ArgumentNullException("view");
			
			bool success = false;
			try {
				if (currentView != view) {
					if (currentView == null) {
						SwitchedToView(view);
					} else {
						try {
							inLoadOperation = true;
							using (Stream sourceStream = OpenRead()) {
								view.Load(this, sourceStream);
							}
						} finally {
							inLoadOperation = false;
						}
					}
				}
				success = true;
			} finally {
				// Only in case of exceptions:
				// (try-finally with bool is better than try-catch-rethrow because it causes the debugger to stop
				// at the original error location, not at the rethrow)
				if (!success) {
					view.Dispose();
				}
			}
		}
		
		/// <summary>
		/// Gets the list of view contents registered with this opened file.
		/// </summary>
		public abstract IList<IViewContent> RegisteredViewContents {
			get;
		}
		
		/// <summary>
		/// Gets the view content that currently edits this file.
		/// If there are multiple view contents registered, this returns the view content that was last
		/// active. The property might return null even if view contents are registered if the last active
		/// content was closed. In that case, the file is stored in-memory and loaded when one of the
		/// registered view contents becomes active.
		/// </summary>
		public IViewContent CurrentView {
			get { return currentView; }
		}
		
		/// <summary>
		/// Opens the file for reading.
		/// </summary>
		public virtual Stream OpenRead()
		{
			if (fileData != null) {
				return new MemoryStream(fileData, false);
			} else {
				return new FileStream(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			}
		}
		
		/// <summary>
		/// Sets the internally stored data to the specified byte array.
		/// This method should only be used when there is no current view or by the
		/// current view.
		/// </summary>
		/// <remarks>
		/// Use this method to specify the initial file content if you use a OpenedFile instance
		/// for a file that doesn't exist on disk but should be automatically created when a view
		/// with the file is saved, e.g. for .resx files created by the forms designer.
		/// </remarks>
		public virtual void SetData(byte[] fileData)
		{
			if (fileData == null)
				throw new ArgumentNullException("fileData");
			if (inLoadOperation)
				throw new InvalidOperationException("SetData cannot be used while loading");
			if (inSaveOperation)
				throw new InvalidOperationException("SetData cannot be used while saving");
			
			this.fileData = fileData;
		}
		
		/// <summary>
		/// Save the file to disk using the current name.
		/// </summary>
		public virtual void SaveToDisk()
		{
			if (IsUntitled)
				throw new InvalidOperationException("Cannot save an untitled file to disk!");
			
			LoggingService.Debug("Save " + FileName);
			bool safeSaving = SD.FileService.SaveUsingTemporaryFile && File.Exists(FileName);
			string saveAs = safeSaving ? FileName + ".bak" : FileName;
			using (FileStream fs = new FileStream(saveAs, FileMode.Create, FileAccess.Write)) {
				if (currentView != null) {
					SaveCurrentViewToStream(fs);
				} else {
					fs.Write(fileData, 0, fileData.Length);
				}
			}
			if (safeSaving) {
				DateTime creationTime = File.GetCreationTimeUtc(FileName);
				File.Delete(FileName);
				try {
					File.Move(saveAs, FileName);
				} catch (UnauthorizedAccessException) {
					// sometime File.Move raise exception (TortoiseSVN, Anti-vir ?)
					// try again after short delay
					System.Threading.Thread.Sleep(250);
					File.Move(saveAs, FileName);
				}
				File.SetCreationTimeUtc(FileName, creationTime);
			}
			IsDirty = false;
		}
		
//		/// <summary>
//		/// Called before saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
//		/// </summary>
//		public event EventHandler SavingCurrentView;
//
//		/// <summary>
//		/// Called after saving the current view. This event is raised both when saving to disk and to memory (for switching between views).
//		/// </summary>
//		public event EventHandler SavedCurrentView;
		
		
		void SaveCurrentViewToStream(Stream stream)
		{
//			if (SavingCurrentView != null)
//				SavingCurrentView(this, EventArgs.Empty);
			inSaveOperation = true;
			try {
				currentView.Save(this, stream);
			} finally {
				inSaveOperation = false;
			}
//			if (SavedCurrentView != null)
//				SavedCurrentView(this, EventArgs.Empty);
		}
		
		protected void SaveCurrentView()
		{
			using (MemoryStream memoryStream = new MemoryStream()) {
				SaveCurrentViewToStream(memoryStream);
				fileData = memoryStream.ToArray();
			}
		}
		
		
		public void SwitchedToView(IViewContent newView)
		{
			if (newView == null)
				throw new ArgumentNullException("newView");
			if (currentView == newView)
				return;
			if (currentView != null) {
				if (newView.SupportsSwitchToThisWithoutSaveLoad(this, currentView)
				    || currentView.SupportsSwitchFromThisWithoutSaveLoad(this, newView))
				{
					// switch without Save/Load
					currentView.SwitchFromThisWithoutSaveLoad(this, newView);
					newView.SwitchToThisWithoutSaveLoad(this, currentView);
					
					currentView = newView;
					return;
				}
				SaveCurrentView();
			}
			try {
				inLoadOperation = true;
				Properties memento = GetMemento(newView);
				using (Stream sourceStream = OpenRead()) {
					IViewContent oldView = currentView;
					bool success = false;
					try {
						currentView = newView;
						// don't reset fileData if the file is untitled, because OpenRead() wouldn't be able to read it otherwise
						if (this.IsUntitled == false)
							fileData = null;
						newView.Load(this, sourceStream);
						success = true;
					} finally {
						// Use finally instead of catch+rethrow so that the debugger
						// breaks at the original crash location.
						if (!success) {
							// stay with old view in case of exceptions
							currentView = oldView;
						}
					}
				}
				RestoreMemento(newView, memento);
			} finally {
				inLoadOperation = false;
			}
		}
		
		public virtual void ReloadFromDisk()
		{
			var r = FileUtility.ObservedLoad(ReloadFromDiskInternal, FileName);
			if (r == FileOperationResult.Failed) {
				if (currentView != null && currentView.WorkbenchWindow != null) {
					currentView.WorkbenchWindow.CloseWindow(true);
				}
			}
		}
		
		void ReloadFromDiskInternal()
		{
			fileData = null;
			if (currentView != null) {
				try {
					inLoadOperation = true;
					Properties memento = GetMemento(currentView);
					using (Stream sourceStream = OpenRead()) {
						currentView.Load(this, sourceStream);
					}
					IsDirty = false;
					RestoreMemento(currentView, memento);
				} finally {
					inLoadOperation = false;
				}
			}
		}
		
		static Properties GetMemento(IViewContent viewContent)
		{
			IMementoCapable mementoCapable = viewContent.GetService<IMementoCapable>();
			if (mementoCapable == null) {
				return null;
			} else {
				return mementoCapable.CreateMemento();
			}
		}
		
		static void RestoreMemento(IViewContent viewContent, Properties memento)
		{
			if (memento != null) {
				((IMementoCapable)viewContent).SetMemento(memento);
			}
		}
	}
	 */
}
